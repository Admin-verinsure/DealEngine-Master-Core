NReco.PdfGenerator - .NET wrapper for wkhtmltopdf
-------------------------------------------------
Visit http://www.nrecosite.com/pdf_generator_net.aspx for the latest information (changes log, examples, etc)
Online API documentation: http://www.nrecosite.com/doc/NReco.PdfGenerator/

Latest version
--------------
NReco.PdfGenerator.dll (embeds wkhtmltopdf binaries, windows-only): https://www.nuget.org/packages/NReco.PdfGenerator/
NReco.PdfGenerator.LT.dll (can be used in .NET Core / Mono projects): https://www.nuget.org/packages/NReco.PdfGenerator.LT/

Examples
--------
NReco.PdfGenerator.Examples.DataSetReport			generates PDF with data table from DataSet using XSLT (console application)
NReco.PdfGenerator.Examples.DemoMvc 				generates PDF by MVC templates. Illustrates: page breaks, multi-page tables, cover page and table-of-contents, custom header/footer with page numbering
NReco.PdfGenerator.Examples.DemoWebForms 			simple demo that generates PDF from user input
NReco.PdfGenerator.Examples.EncryptPdf				how to generate PDF and protect it with password
NReco.PdfGenerator.Examples.PdfFromFiles			how to generate PDF from several HTML documents (files, URLs)
NReco.PdfGenerator.Examples.WatermarkPdf			how to generate PDF and watermark it with background image
NReco.PdfGenerator.Examples.BatchMode				generates several PDF files in batch mode
NReco.PdfGenerator.Examples.MergeMode				produce several PDF results and merge them into one file with iTextSharp
NReco.PdfGenerator.Examples.WordToHtml				converts Word docx file (2007+, OpenXML only) to PDF

License
-------
All binary components and source code in this package are covered by LICENSE (license.txt file).

PdfGenerator uses WkHtmlToPdf command line tool (distributed under LGPL, source code: https://github.com/wkhtmltopdf/wkhtmltopdf )