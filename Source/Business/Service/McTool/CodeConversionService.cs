namespace App;

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Tool.Compet.Core;

public class CodeConversionService {
	private static Dictionary<string, string> _InchMap = new();

	/// Take care when change since `inchSeparatorChars` and `inchSeparatorRegex` must be same !!
	private static readonly char[] inchSeparatorChars = "X*,".ToCharArray();

	private static readonly string inchSeparatorRegex = "[X,\\*,\\,]";
	private static readonly string _InchSubPattern = "[\\d\\.]+[\\d\\.\\s'\"\\/-]*"; // .75" - 1/4'
	private static readonly string _InchFullPatter = $"({_InchSubPattern})" + "(\\s{0,1}" + inchSeparatorRegex + "\\s{0,1}" + _InchSubPattern + "){0,2}";
	private static readonly Regex _InchRegex = new(_InchFullPatter, RegexOptions.Compiled);

	private static DkKeyOrderedHashMap<string> _SizeMap = new();

	private static readonly HashSet<string> _SupportedCodeIdentifiers = new() {
		"ELB",
		"ECC",
		"CAP",
		"CON",
		"TEE",
		"RED"
	};

	/// CustomerCode -> MyCode. For eg,. ELL -> ELB, ELBOW -> ELB
	private static Dictionary<string, CodeNode> _CodeMap = new();

	private static Dictionary<string, string> _ElbowShortLongMap = new();

	/// Replace occurrencies to target text before jump to process.
	private static Dictionary<string, string> _Replacement = new();

	private static Dictionary<string, string> _Blacklist = new();

	static CodeConversionService() {
	}

	public static async Task<ApiResponse> LoadSetting(string? settingFilePath = null) {
		settingFilePath ??= AppConst.MCTOOL_SETTING_FILE_RELATIVE_PATH;
		var settingData = await ExcelHelper.ReadExcelAsync(settingFilePath);
		if (settingData is null) {
			throw new InvalidDataException("Could not read setting");
		}

		var tmpReplacementMap = new Dictionary<string, string>();
		var tmpCodeMap = new Dictionary<string, CodeNode>();
		var tmpSizeNodes = new Dictionary<string, SizeMapNode>();
		var tmpInchMaps = new Dictionary<string, string>();
		var tmpShortLongMap = new Dictionary<string, string>();
		var tmpBlacklist = new Dictionary<string, string>();

		foreach (var sheet in settingData.sheets) {
			var sheetNameLower = sheet.name.ToLower(CultureInfo.InvariantCulture);

			if (sheetNameLower == "replacement") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var fromText = row[0].Trim().ToUpper(CultureInfo.InvariantCulture);
					var toText = row[1].Trim().ToUpper(CultureInfo.InvariantCulture);
					if (fromText.Length == 0) {
						if (toText.Length == 0) {
							continue;
						}
						throw new InvalidDataException($"FromText must not empty for ToText: {toText}");
					}

					// Register replacement map
					tmpReplacementMap.Add(fromText, toText);
				}
			}
			else if (sheetNameLower == "codemap") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var fromCode = row[0].Trim().ToUpper(CultureInfo.InvariantCulture); // ELBOW
					var toCode = row[1].Trim().ToUpper(CultureInfo.InvariantCulture); // E
					var codeIdentifier = row[2].Trim().ToUpper(CultureInfo.InvariantCulture); // ELB
					if (fromCode.Length == 0) {
						if (toCode.Length == 0) {
							continue;
						}
						throw new Exception($"FromCode must not empty for ToCode: {toCode}");
					}
					if (!_SupportedCodeIdentifiers.Contains(codeIdentifier)) {
						throw new Exception($"Not support code: {toCode}");
					}

					// Register code map
					tmpCodeMap.Add(fromCode,
						new() {
							fromCode = fromCode,
							toCode = toCode,
							codeIdentifier = codeIdentifier
						});
				}
			}
			else if (sheetNameLower == "sizemap") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var fromSize = row[0].Trim().ToUpper(CultureInfo.InvariantCulture);
					var toSize = row[1].Trim().ToUpper(CultureInfo.InvariantCulture);
					if (fromSize.Length == 0) {
						if (toSize.Length == 0) {
							continue;
						}
						throw new Exception($"FromSize must not empty for ToSize: {toSize}");
					}

					// Register size node
					tmpSizeNodes.Add(fromSize,
						new SizeMapNode {
							size = toSize
						});
				}
			}
			else if (sheetNameLower == "inchmap") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var fromInch = row[0].Trim().ToUpper(CultureInfo.InvariantCulture);
					var toInch = row[1].Trim().ToUpper(CultureInfo.InvariantCulture);
					if (fromInch.Length == 0) {
						if (toInch.Length == 0) {
							continue;
						}
						throw new Exception($"FromInch must not empty for ToInch: {toInch}");
					}

					// Register inch conversion
					tmpInchMaps.Add(fromInch, toInch);
				}
			}
			else if (sheetNameLower == "elb_shortlong") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var fromName = row[0].Trim().ToUpper(CultureInfo.InvariantCulture);
					var toName = row[1].Trim().ToUpper(CultureInfo.InvariantCulture);
					if (fromName.Length == 0) {
						if (toName.Length == 0) {
							continue;
						}
						throw new Exception($"FromName must not empty for ToName: {toName}");
					}

					// Register size node
					tmpShortLongMap.Add(fromName, toName);
				}
			}
			else if (sheetNameLower == "blacklist") {
				var first = true;

				foreach (var row in sheet.table!) {
					// Skip header
					if (first) {
						first = false;
						continue;
					}

					var text = row[0].Trim().ToUpper(CultureInfo.InvariantCulture);
					var description = row[1].Trim().ToUpper(CultureInfo.InvariantCulture);
					if (text.Length == 0) {
						if (description.Length == 0) {
							continue;
						}
						throw new Exception($"Text must not empty for Description: {description}");
					}

					// Register blacklist for pre-processing
					tmpBlacklist.Add(text, description);
				}
			}
			else {
				throw new Exception($"Not support sheet: {sheet.name}");
			}
		}

		// Console.WriteLine("---> Done reload setting !");

		_Replacement = tmpReplacementMap;
		_CodeMap = tmpCodeMap;
		_SizeMap = _BuildSizeMap(tmpSizeNodes);
		_InchMap = tmpInchMaps;
		_ElbowShortLongMap = tmpShortLongMap;
		_Blacklist = tmpBlacklist;

		return ApiResponse.Ok;
	}

	private static DkKeyOrderedHashMap<string> _BuildSizeMap(Dictionary<string, SizeMapNode> inSizeMap) {
		var outSizeMap = new DkKeyOrderedHashMap<string>();
		var keys = inSizeMap.Keys.ToArray();

		// Build tree
		for (var i = keys.Length - 1; i >= 0; --i) {
			for (var jj = i - 1; jj >= 0; --jj) {
				if (keys[i].Contains(keys[jj])) {
					inSizeMap[keys[i]].children.Add(keys[jj]);
				}
				else if (keys[jj].Contains(keys[i])) {
					inSizeMap[keys[jj]].children.Add(keys[i]);
				}
			}
		}

		// Traversal key and update priority for nodes.
		// The key that contains other key will have high priority than its children.
		// We make a graph, then just dp to calculate priority (depth) of each node.
		foreach (var key in keys) {
			_CalcPriorityForSizeNode(inSizeMap, key);
		}

		Array.Sort(keys,
			(s1, s2) => {
				return inSizeMap[s2].priority - inSizeMap[s1].priority;
			});

		foreach (var key in keys) {
			outSizeMap.Add(key, inSizeMap[key].size);
		}

		return outSizeMap;
	}

	private static int _CalcPriorityForSizeNode(Dictionary<string, SizeMapNode> sizeMap, string parent) {
		var priority = sizeMap[parent].priority;
		if (priority >= 0) {
			return priority;
		}
		priority = 0;
		foreach (var child in sizeMap[parent].children) {
			priority = Math.Max(priority, 1 + _CalcPriorityForSizeNode(sizeMap, child));
		}
		sizeMap[parent].priority = priority;

		return priority;
	}

	public async Task<ExcelData> ConvertCodeFromFile(string filePath) {
		var data = await ExcelHelper.ReadExcelAsync(filePath, replaceMultipleSpaces: true);

		foreach (var sheet in data.sheets) {
			var table = sheet.table!;
			var rowCount = table.Length;

			for (var rowIdx = 0; rowIdx < rowCount; ++rowIdx) {
				var rowArr = table[rowIdx];
				var colCount = rowArr.Length;
				var newRow = new List<string>(colCount * 3);

				for (var col = 0; col < colCount; ++col) {
					var convertedColumn = this.ConvertDescription(rowArr[col]);
					newRow.Add(rowArr[col]);
					newRow.Add(convertedColumn);
					newRow.Add(convertedColumn.Length + string.Empty);
				}

				table[rowIdx] = newRow.ToArray();
			}
		}

		return data;
	}

	public string ConvertDescription(string _description) {
		var description = _description.ToUpper(CultureInfo.InvariantCulture);

		// Check blacklist
		foreach (var pair in _Blacklist) {
			if (description.Contains(pair.Key)) {
				return $"invalid_description ({_description}), has blacklist: ({pair.Value})";
			}
		}

		// Pre-process (replacement)
		description = this._PreprocessDescription(description);
		// Console.WriteLine($"---> before: {_description} -> after: {description}");

		// Calculate code
		CodeNode? myCode = null;
		foreach (var customerCode in _CodeMap.Keys) {
			if (description.Contains(customerCode)) {
				myCode = _CodeMap[customerCode];
				break;
			}
		}

		if (myCode is null) {
			return $"[invalid_description:not_contain_supported_code ({_description})]";
		}

		switch (myCode.codeIdentifier) {
			case "ELB":
				return this._ConvertElb(description, myCode);
			case "TEE":
				return this._ConvertTee(description, myCode.toCode);
			case "CAP":
				return this._ConvertCap(description, myCode.toCode);
			case "CON":
				return this._ConvertCon(description, myCode.toCode);
			case "ECC":
				return this._ConvertEcc(description, myCode.toCode);
			case "RED":
				return this._ConvertRed(description, myCode.toCode);
		}

		return $"[invalid_description:not_contain_supported_code ({_description})]";
	}

	private string _PreprocessDescription(string _description) {
		var description = _description;

		foreach (var pair in _Replacement) {
			description = description.Replace(pair.Key, pair.Value);
		}

		return description;
	}

	/// .5 STD LR 45 ELL WPB
	/// 1" STD CODO 90° L.R. SIN SOLD. BW A-234 WPB
	/// 1" STD CODO 90° S.R. SIN SOLD. BW A-234 WPB
	private string _ConvertElb(string description, CodeNode myCode) {
		var ans = myCode.toCode;

		// Require Size before calc inches
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Require Inches
		var inchesResult = this._CalcInches(description, singlePatternPadCount: 2);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}

		// Convert xE -> R if found 2 inches dimension
		var isR = inchesResult.partCount == 2;
		if (isR) {
			ans = "R";
		}

		// Do not append Long or Short if meet one of below case:
		// - Is R (elbow that 2 dimensions)
		// - FromCode is 45
		if (!isR && myCode.fromCode != "45") {
			foreach (var key in _ElbowShortLongMap.Keys) {
				if (description.Contains(key)) {
					ans += _ElbowShortLongMap[key];
					break;
				}
			}
		}

		ans += (sizeResult.size + inchesResult.inches);

		return ans;
	}

	private string _ConvertRed(string description, string toCode) {
		var ans = toCode;

		// Require Size
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		ans += sizeResult.size;
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Require Inches
		var inchesResult = this._CalcInches(description, singlePatternPadCount: 2);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}
		ans += inchesResult.inches;

		return ans;
	}

	/// .75 X .5 STD TEE WPB
	/// .5 XH TEE WPB
	/// 5"X5"X2-1/2 CS STD WLD TEE
	/// 5"X2.1/2" REDUCING TEE STD ASTM A234 WPB ASME B16.9
	private string _ConvertTee(string description, string toCode) {
		var ans = toCode;

		// Require Size
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		ans += sizeResult.size;
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Inches
		var inchesResult = this._CalcInches(description);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}
		ans += inchesResult.inches;

		return ans;
	}

	/// ECC 4" X 2" STD/ S160
	/// ECC RED BW 10"x8" SCH 30/20 ASTM A420 WPL6 SMLS
	/// Eccentric Reducer 10" * 8" S140/W.T. 25.4mm
	/// ECC1-1/2"*3/4"STD
	private string _ConvertEcc(string description, string toCode) {
		var ans = toCode;

		// Require Size
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		ans += sizeResult.size;
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Inches
		var inchesResult = this._CalcInches(description);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}
		ans += inchesResult.inches;

		return ans;
	}

	/// CON RED BW 1 1/2"x1" SCH XS/XS ASTM A234 WPB SMLS
	/// CONCENTRIC REDUC. ASTM B16.9 STD WPB 8"X5"
	/// Concentric Reducer 6" * 3" W.T. 5.56 * 3.18mm
	/// Concentric Reducer 6" * 3" XH/STD
	private string _ConvertCon(string description, string toCode) {
		var ans = toCode;

		// Require Size
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		ans += sizeResult.size;
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Inches
		var inchesResult = this._CalcInches(description);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}
		ans += inchesResult.inches;

		return ans;
	}

	/// CAP 1" STD
	/// CAP 10" W.T. 38.1mm
	/// CAP 1-1/2" S160
	/// CAP 12" W.T. 31.75mm
	private string _ConvertCap(string description, string toCode) {
		var ans = toCode;

		// Require Size
		var sizeResult = this._CalcSize(description);
		if (sizeResult.size.Length == 0) {
			return $"invalid_size:no_size ({description})";
		}
		ans += sizeResult.size;
		description = sizeResult.descriptionAfterRemovedTheSize;

		// Inches
		var inchesResult = this._CalcInches(description);
		if (inchesResult.inches.Length == 0) {
			return $"invalid_inches:no_inches ({description}) ({inchesResult.message})";
		}
		ans += inchesResult.inches;

		return ans;
	}

	private SizeResult _CalcSize(string description, bool resultRequired = true) {
		foreach (var key in _SizeMap.Keys) {
			var firstIndex = description.IndexOf(key);
			if (firstIndex >= 0) {
				return new SizeResult {
					size = _SizeMap[key],
					descriptionAfterRemovedTheSize = description.Remove(firstIndex, key.Length)
				};
			}
		}

		return new SizeResult {
			size = string.Empty,
			descriptionAfterRemovedTheSize = description
		};
	}

	private InchesResult _CalcInches(string text, int singlePatternPadCount = 4) {
		var result = new InchesResult();
		var matches = _InchRegex.Matches(text);
		var matchCount = matches.Count;

		var bestResult = string.Empty;
		var partCount = 0;
		var bestPrecision = -1;
		var foundPatternCount = 0;
		var message = string.Empty;

		// Console.WriteLine($"From text: {text}");

		for (var index = 0; index < matchCount; ++index) {
			var match = matches[index];
			// Console.WriteLine($"  -> Found inches match: {match.Value}");
			var target = match.Value;
			var maybeInches = target.Split(inchSeparatorChars);

			// Console.WriteLine("---> maybeInches: " + string.Join("___", maybeInches));

			// Get valid inches
			var validInches = new List<InchesDetectionResult>();
			foreach (var maybeInch in maybeInches) {
				var inchResult = this._TryCalcInchesPartAndConvertToOurCode(maybeInch);
				if (inchResult.value.Length > 0) {
					validInches.Add(inchResult);
				}
			}

			// Console.WriteLine("---> validInches: " + string.Join("___", validInches.Select(m => m.value)));

			if (validInches.Count == 1) {
				var inches = validInches[0];
				message += inches.message;

				if (inches.value.Length > 0) {
					++foundPatternCount;
					if (bestPrecision <= inches.precision) {
						bestPrecision = inches.precision;
						bestResult = inches.value.PadLeft(singlePatternPadCount, '0'); // Format 00xy
						partCount = 1;
					}
				}
			}
			else if (validInches.Count == 2) {
				var inches1 = validInches[0];
				var inches2 = validInches[1];
				message += inches1.message;
				message += inches2.message;

				if (inches1.value.Length > 0 && inches2.value.Length > 0) {
					++foundPatternCount;
					var bothPrecision = Math.Max(inches1.precision, inches2.precision);
					if (bestPrecision <= bothPrecision) {
						bestPrecision = bothPrecision;
						bestResult = inches1.value.PadLeft(2, '0') + inches2.value.PadLeft(2, '0'); // Format 0x0y
						partCount = 2;
					}
				}
			}
			// Normally it is TEE, just take element at 1 and 3.
			else if (validInches.Count == 3) {
				var inches1 = validInches[0];
				var inches2 = validInches[2];
				message += inches1.message;
				message += inches2.message;

				if (inches1.value.Length > 0 && inches2.value.Length > 0) {
					++foundPatternCount;
					var bothPrecision = Math.Max(inches1.precision, inches2.precision);
					if (bestPrecision <= bothPrecision) {
						bestPrecision = bothPrecision;
						bestResult = inches1.value.PadLeft(2, '0') + inches2.value.PadLeft(2, '0'); // Format 0x0y
						partCount = 3;
					}
				}
			}
		}

		// Console.WriteLine($"-> Inches dim: {bestResult}");

		if (foundPatternCount > 1) {
			message += $"[invalid_inches:too_much_valid_pattern:{foundPatternCount} ({text})]";
		}

		return new InchesResult {
			inches = bestResult,
			partCount = partCount,
			message = message
		};
	}

	/// Calculate inches, then Convert to our code (2 digits).
	/// Inches:
	/// .5
	/// 3/4'
	/// 1.1/4'
	/// 1 1/4
	/// 1.5 -> 1 + 1/2 = 10 + 4 = 14
	/// 1.25
	/// 24" . .75
	/// 3/4' - 1/4'
	/// .25" . 1/4'
	/// 0.75' . 1"
	/// 24" . 3/4
	/// 24'.75"
	private InchesDetectionResult _TryCalcInchesPartAndConvertToOurCode(string _inches) {
		// Console.WriteLine($"    -> Try convert inches: {_inches}");

		var inches = _inches.Trim();
		var N = inches.Length;

		// Parse inches from given _inches
		var numbers = new List<InchesDetectionResult>();
		var curNumber = string.Empty;

		for (var index = 0; index < N; ++index) {
			var ch = inches[index];

			// Digit
			if (DkMaths.IsDigit(ch)) {
				curNumber += ch;
			}
			// Number 1.3/4
			else if (ch == '/') {
				// Require number before
				if (curNumber.Length == 0) {
					break;
				}
				curNumber += '/';
			}
			// Inches
			else if (ch == '\'' || ch == '"') {
				// Require number before
				if (curNumber.Length == 0) {
					break;
				}
				// Accept current number
				this._AddInchIfValid(numbers: numbers, _curNumber: curNumber, precision: Precision.High);
				curNumber = string.Empty;
			}
			// Number separator
			else if (ch == '-' || ch == ' ') {
				// Accept current number
				if (curNumber.Length > 0) {
					this._AddInchIfValid(numbers: numbers, _curNumber: curNumber);
					curNumber = string.Empty;
				}
			}
			// Maybe number-separator (1.1/4 = 1.25, 1.1/2 = 1.5, 1.3/4 = 1.75), decimal (.5, 0.75)
			else if (ch == '.') {
				// The ch is maybe decimal
				if (curNumber == string.Empty || curNumber == "0") {
					if (this._HasNextInchesDecimalValueFromDot(inches, index)) {
						curNumber += '.';
					}
					// Current char should be separator
					else {
						if (numbers.Count == 0) {
							break;
						}
					}
				}
				// The ch is separator
				else if (curNumber.Length > 0) {
					this._AddInchIfValid(numbers: numbers, _curNumber: curNumber);
					curNumber = string.Empty;

					// Check to accept dot or not
					if (this._HasNextInchesDecimalValueFromDot(inches, index)) {
						curNumber += '.';
					}
				}
			}
		}

		if (curNumber.Length > 0) {
			this._AddInchIfValid(numbers: numbers, _curNumber: curNumber);
			curNumber = string.Empty;
		}

		// Only accept inches of 1 or 2 numbers
		if (numbers.Count < 1 || numbers.Count > 2) {
			// Console.WriteLine($"    -> Wrong part count, numbers: [{string.Join("___", numbers.Select(m => m.value))}]");
			return new InchesDetectionResult() {
				message = $"[invalid_inches:wrong_part_count ({inches})]"
			};
		}

		var result = new InchesDetectionResult();

		foreach (var number in numbers) {
			var codeStr = _InchMap[number.value];
			if (int.TryParse(codeStr, out var codeInt)) {
				if (int.TryParse(result._tmpSum, out var tmpSum)) {
					result._tmpSum = (tmpSum + codeInt) + string.Empty;
				}
				else {
					result._tmpSum += codeInt;
				}
			}
			else {
				result._tmpSum += codeStr;
			}
			result.precision += number.precision;
		}
		result.value = result._tmpSum;

		// Console.WriteLine($"    -> Got inches code: {inches} -> {result.value}");

		return result;
	}

	/// Example: 3/4, .75
	/// Trim for convenience: 0.75 -> .75
	private bool _AddInchIfValid(List<InchesDetectionResult> numbers, string _curNumber, int precision = Precision.Low) {
		var number = _curNumber.Trim().TrimStart('0');

		if (_InchMap.ContainsKey(number)) {
			// Console.WriteLine($"---> _AddInchIfValid: accepted _curNumber: {_curNumber}, number: {number}");
			numbers.Add(new() {
				value = number,
				precision = precision
			});
			return true;
		}

		// Console.WriteLine($"---> _AddInchIfValid: ignore add _curNumber: {_curNumber}, number: {number}");

		return false;
	}

	private bool _HasNextInchesDecimalValueFromDot(string inches, int dotIndex) {
		var number = ".";

		for (var index = dotIndex + 1; index < inches.Length; ++index) {
			var ch = inches[index];
			if (!DkMaths.IsDigit(ch)) {
				break;
			}
			number += ch;
		}

		return _InchMap.ContainsKey(number);
	}

	internal class SizeMapNode {
		internal string size = string.Empty;
		internal HashSet<string> children = new();
		internal int priority = -1;
	}

	internal class CodeNode {
		internal string fromCode = string.Empty;
		internal string toCode = string.Empty;
		internal string codeIdentifier = string.Empty;
	}

	internal class Precision {
		internal const int Low = 1;
		internal const int Medium = 5;
		internal const int High = 10;
	}

	internal class InchesDetectionResult {
		internal string value = string.Empty;
		internal int precision = Precision.Low;
		internal string message = string.Empty;
		internal string _tmpSum = string.Empty;
	}

	class InchesResult {
		public string inches = string.Empty;
		public int partCount;
		public string message = string.Empty;
	}

	class SizeResult {
		public string size = string.Empty;
		public string descriptionAfterRemovedTheSize = string.Empty;
	}
}
