using System.Globalization;

namespace BlazorBudget.Web.Data;

    static class IngStatementParser
    {
        public static CsvStatementParser For(bool withBalance = true, bool withNotes = true)
        {
            var numHeaderRows = 13;
            var offsets = new ColumnOffsets(Date: 0, Amount: 5, Type: 3, Recipient: 2, Reference: 4);

            if (withBalance)
            {
                numHeaderRows++;
                offsets = offsets with { Amount = offsets.Amount + 2 };
            }

            if (withNotes)
            {
                offsets = offsets with { Amount = offsets.Amount + 1, Reference = offsets.Reference + 1 };
            }

            return new CsvStatementParser(numHeaderRows, offsets, CultureInfo.CreateSpecificCulture("de-DE"));
        }
    }

