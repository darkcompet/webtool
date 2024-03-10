namespace App;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

public class ExcelData {
	public List<Sheet> sheets = new();

	public class Sheet {
		public string name;
		public string[]? header;
		public string[][]? table;
	}
}

public class ExcelHelper {
	public static async Task<ExcelData> ReadExcelAsync(string filePath, bool trim = true, bool replaceMultipleSpaces = false) {
		var excelData = new ExcelData();

		await Task.Run(() => {
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using (var excelPack = new ExcelPackage()) {
				// Read-only excel file as stream
				using (var stream = File.OpenRead(filePath)) {
					excelPack.Load(stream);
				}

				// Read on each sheet
				foreach (var sheet in excelPack.Workbook.Worksheets) {
					// Cells is all dimension (1048576 x 16384), not Content Dimension.
					// Dimension is actual data in the sheet.
					var cells = sheet.Cells;
					var dimension = sheet.Dimension;
					if (dimension is null) {
						continue;
					}
					var startCell = dimension.Start;
					var endCell = dimension.End;

					var baseRow = startCell.Row;
					var baseColumn = startCell.Column;
					var rowCount = endCell.Row - baseRow + 1;
					var colCount = endCell.Column - baseColumn + 1;
					var table = new string[rowCount][];

					for (var row = baseRow; row <= endCell.Row; ++row) {
						table[row - baseRow] = new string[colCount];

						for (var col = baseColumn; col <= endCell.Column; ++col) {
							var text = cells[row, col].Text;
							// Trim whitespace
							if (trim) {
								text = text.Trim();
							}
							// Remove 2+ spaces to 1 space
							if (replaceMultipleSpaces) {
								text = Regex.Replace(text, "\\s{2,}", " ");
							}
							table[row - baseRow][col - baseColumn] = text;
						}
					}

					// Add new sheet
					excelData.sheets.Add(new() {
						name = sheet.Name,
						table = table
					});

					// // Debug table of the sheet
					// foreach (var row in table) {
					// 	Console.Write("[");
					// 	var first = true;
					// 	foreach (var col in row) {
					// 		if (first) {
					// 			first = false;
					// 			Console.Write($"{col}");
					// 		}
					// 		else {
					// 			Console.Write($", {col}");
					// 		}
					// 	}
					// 	Console.WriteLine("]");
					// }
				}
			}
		});

		return excelData;
	}

	public static void WriteToExcelFile(string filePath, ExcelData data) {
		using (var excelPack = new ExcelPackage()) {
			foreach (var sheet in data.sheets) {
				var workSheet = excelPack.Workbook.Worksheets.Add(sheet.name);

				// Setup worksheet
				workSheet.TabColor = System.Drawing.Color.Black;
				workSheet.DefaultRowHeight = 12;

				var curRowPos = 1;
				var maxColCount = 0;

				// Fill header of the sheet
				if (sheet.header != null) {
					var headerRow = workSheet.Row(curRowPos);
					headerRow.Height = 20;
					headerRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
					headerRow.Style.Font.Bold = true;

					var colCount = sheet.header.Length;
					for (var col = 0; col < colCount; ++col) {
						workSheet.Cells[1, col + 1].Value = sheet.header[col];
					}

					++curRowPos;
					maxColCount = Math.Max(maxColCount, colCount);
				}

				// Fill data
				var table = sheet.table;
				if (table != null) {
					var rowCount = table.Length;
					for (var row = 0; row < rowCount; ++row) {
						var colCount = table[row].Length;
						for (var col = 0; col < colCount; ++col) {
							workSheet.Cells[curRowPos, col + 1].Value = table[row][col];
						}
						++curRowPos;
						maxColCount = Math.Max(maxColCount, colCount);
					}
				}

				// Autofit content
				for (var col = 1; col <= maxColCount; ++col) {
					workSheet.Column(col).AutoFit();
				}
			}

			// Created all sheets, now start write to file
			var fileStream = File.Create(filePath);
			fileStream.Close();
			File.WriteAllBytes(filePath, excelPack.GetAsByteArray());
		}
	}

	// private void test_parse_excel_DOM() {
	// 	// Ref: https://learn.microsoft.com/en-us/office/open-xml/how-to-parse-and-read-a-large-spreadsheet
	// 	// Open the document as read-only.
	// 	using (var spreadsheetDocument = SpreadsheetDocument.Open("local/test.xlsx", false)) {
	// 		var workbookPart = spreadsheetDocument.WorkbookPart;
	// 		if (workbookPart is null) {
	// 			return;
	// 		}

	// 		var worksheetPart = workbookPart.WorksheetParts.First();
	// 		var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

	// 		foreach (var row in sheetData.Elements<Row>()) {
	// 			foreach (Cell cell in row.Elements<Cell>()) {
	// 				Console.Write(cell.CellValue.Text + ", ");
	// 			}
	// 		}
	// 	}
	// }

	// private void test_parse_excel_SAX() {
	// 	// Ref: https://learn.microsoft.com/en-us/office/open-xml/how-to-parse-and-read-a-large-spreadsheet
	// 	// Open the document as read-only.
	// 	using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open("local/test.xlsx", false)) {
	// 		var workbookPart = spreadsheetDocument.WorkbookPart;
	// 		var worksheetPart = workbookPart.WorksheetParts.First();

	// 		var reader = OpenXmlReader.Create(worksheetPart);
	// 		while (reader.Read()) {
	// 			if (reader.ElementType == typeof(CellValue)) {
	// 				var text = reader.GetText();
	// 				Console.Write(text + " ");
	// 			}
	// 		}
	// 	}
	// }


	// private void test_parse_excel_OPENXML() {
	// 	// Ref: https://learn.microsoft.com/en-us/office/open-xml/how-to-parse-and-read-a-large-spreadsheet
	// 	// Src: https://gist.github.com/kzelda/2facdff2d924349fe96c37eab0e9ee47
	// 	try {
	// 		using (var doc = SpreadsheetDocument.Open("local/test4.xlsx", false)) {
	// 			var workbookPart = doc.WorkbookPart;
	// 			if (workbookPart is null) {
	// 				return;
	// 			}

	// 			var sheet = workbookPart.Workbook?.Sheets?.GetFirstChild<Sheet>();
	// 			var sheetId = sheet?.Id?.Value;
	// 			if (sheet is null || sheetId is null) {
	// 				return;
	// 			}

	// 			var worksheet = (workbookPart.GetPartById(sheetId) as WorksheetPart)?.Worksheet;
	// 			var sheetData = worksheet?.GetFirstChild<SheetData>();
	// 			if (sheetData is null) {
	// 				return;
	// 			}

	// 			foreach (var row in sheetData.Descendants<Row>()) {
	// 				foreach (var cell in row.Descendants<Cell>()) {
	// 					var value = GetCellValue(workbookPart, cell);
	// 					Console.Write($"{value}, ");
	// 				}

	// 				// Done row
	// 				Console.WriteLine();
	// 			}
	// 		}
	// 	}
	// 	catch (Exception e) {
	// 		Console.WriteLine($"---------- error: {e.Message}");
	// 	}
	// }

	// private static string? GetCellValue(WorkbookPart workbookPart, Cell cell) {
	// 	var cellValue = cell.CellValue?.InnerText;
	// 	if (cell.DataType != null) {
	// 		if (cell.DataType.Value == CellValues.SharedString) {
	// 			return workbookPart.SharedStringTablePart?.SharedStringTable.ChildElements.GetItem(int.Parse(cellValue!)).InnerText;
	// 		}
	// 		return cellValue;
	// 	}
	// 	return cellValue;
	// }
}
