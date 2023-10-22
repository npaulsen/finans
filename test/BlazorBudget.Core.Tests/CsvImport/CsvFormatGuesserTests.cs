using BlazorBudget.Core.CsvImport;

namespace BlazorBudget.Core.Tests;

public class CsvFormatGuesserTests
{
    [Fact]
    public void Finds_columns_in_DKB_example()
    {
        var splitter = new CsvLineSplitter(";");
        var headers = splitter.Split(""" "Buchungsdatum";"Wertstellung";"Status";"Zahlungspflichtige*r";"Zahlungsempfänger*in";"Verwendungszweck";"Umsatztyp";"Betrag";"Gläubiger-ID";"Mandatsreferenz";"Kundenreferenz" """);
        var exampleColumns = new string[][]{
            splitter.Split(""" "20.10.23";"20.10.23";"Gebucht";"ISSUER";"ALDI SAGT DANKE/KIEL//DE";"2023-10-19 Debitk.19 VISA Debit";"Ausgang";"-10,89 €";"";"";"123" """)
        };

        var mappings = CsvFormatGuesser.GuessColumns(headers, exampleColumns);

        Assert.Equal(1, mappings.Date);
        Assert.Equal(4, mappings.Recipient);
        Assert.Equal(5, mappings.Reference);
        Assert.Equal(6, mappings.Type);
        Assert.Equal(7, mappings.Amount);
    }

    [Fact]
    public void Finds_date_format_in_DKB_example()
    {
        var splitter = new CsvLineSplitter(";");
        var headers = splitter.Split(""" "Buchungsdatum";"Wertstellung";"Status";"Zahlungspflichtige*r";"Zahlungsempfänger*in";"Verwendungszweck";"Umsatztyp";"Betrag";"Gläubiger-ID";"Mandatsreferenz";"Kundenreferenz" """);
        var exampleColumns = new string[][]{
            splitter.Split(""" "20.10.23";"20.10.23";"Gebucht";"ISSUER";"ALDI SAGT DANKE/KIEL//DE";"2023-10-19 Debitk.19 VISA Debit";"Ausgang";"-10,89 €";"";"";"123" """)
        };

        var mappings = CsvFormatGuesser.GuessColumns(headers, exampleColumns);

        Assert.NotNull(mappings.DateFormat);
        Assert.Equal("dd.MM.yy", mappings.DateFormat);
    }

     [Fact]
    public void Finds_number_format_in_DKB_example()
    {
        var splitter = new CsvLineSplitter(";");
        var headers = splitter.Split(""" "Buchungsdatum";"Wertstellung";"Status";"Zahlungspflichtige*r";"Zahlungsempfänger*in";"Verwendungszweck";"Umsatztyp";"Betrag";"Gläubiger-ID";"Mandatsreferenz";"Kundenreferenz" """);
        var exampleColumns = new string[][]{
            splitter.Split(""" "20.10.23";"20.10.23";"Gebucht";"ISSUER";"ALDI SAGT DANKE/KIEL//DE";"2023-10-19 Debitk.19 VISA Debit";"Ausgang";"-10,89 €";"";"";"123" """)
        };

        var mappings = CsvFormatGuesser.GuessColumns(headers, exampleColumns);

        Assert.NotNull(mappings.Culture);
        Assert.Equal("de-DE", mappings.Culture.Name);
    }
}