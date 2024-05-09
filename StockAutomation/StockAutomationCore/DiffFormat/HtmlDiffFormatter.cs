using StockAutomationCore.Diff;

namespace StockAutomationCore.DiffFormat;

public static class HtmlDiffFormatter
{
    public static string TableTemplate => """
                                        <table>
                                            <thead>
                                                <tr>
                                                    <th>Company Name</th>
                                                    <th>Ticker</th>
                                                    <th style='text-align: end;'>Shares</th>
                                                    <th>Diff</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {0}
                                            </tbody>
                                        </table>
                                        """;

    public static string TableRowFormat => """
                                          <tr>
                                              <td>{0}</td>
                                              <td>{1}</td>
                                              <td style='text-align: end;'>{2}</td>
                                              <td>{3}</td>
                                          </tr>
                                          """;

    public static string PositiveDiff => "<span style='color: green;'>\u25b2 +{0:0.00%}</span>";
    public static string NegativeDiff => "<span style='color: red;'>\u25bc {0:0.00%}</span>";

    public static string TableSeparator => """
                                          <tr>
                                              <td colspan='4' style='height: 10px;'></td>
                                          </tr>
                                          """;

    public static string FormatHtml(this HoldingsDiff diff)
    {
        var newPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares == 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => string.Format(TableRowFormat, hdl.CompanyName, hdl.Ticker, hdl.New.Shares, ""))
            .ToList();
        var increasedPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.Old.Shares > 0 && hdl.QuantityDiff > 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => string.Format(TableRowFormat, hdl.CompanyName, hdl.Ticker, hdl.New.Shares, GetDiff(hdl)))
            .ToList();
        var reducedPositions = diff.HoldingsDiffLines.Values
            .Where(hdl => hdl.QuantityDiff < 0)
            .OrderBy(hdl => hdl.CompanyName)
            .Select(hdl => string.Format(TableRowFormat, hdl.CompanyName, hdl.Ticker, hdl.New.Shares, GetDiff(hdl)))
            .ToList();
        if (newPositions.Count == 0 && increasedPositions.Count == 0 && reducedPositions.Count == 0)
        {
            return "<h1>No changes in the index</h1>";
        }

        List<string?> sections =
        [
            string.Join('\n', newPositions),
            string.Join('\n', increasedPositions),
            string.Join('\n', reducedPositions)
        ];
        var tableBody = string.Join(TableSeparator, sections.Where(s => s != null));
        return string.Format(TableTemplate, tableBody);
    }

    private static string GetDiff(HoldingsDiffLine diffLine)
    {
        var diff = decimal.Abs((decimal) diffLine.QuantityDiff / (decimal) diffLine.Old.Shares);
        return diffLine.QuantityDiff > 0 ? string.Format(PositiveDiff, diff) : string.Format(NegativeDiff, diff);
    }
}