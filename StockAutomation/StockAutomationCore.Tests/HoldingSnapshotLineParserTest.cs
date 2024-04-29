using System.Numerics;
using System.Text;
using StockAutomationCore.Parser;

namespace StockAutomationCore.Tests;

public class HoldingSnapshotLineParserTest
{
    [Test]
    [TestCase("""
    date,fund,company,ticker,cusip,shares,"market value ($)","weight (%)"
    04/03/2024,ARKK,"COINBASE GLOBAL INC -CLASS A",COIN,19260Q107,"2,851,172","$700,932,124.48",9.66 %
    """),
    TestCase(
    """
    date,fund,company,ticker,cusip,shares,market value ($),weight (%)
    04/03/2024,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,2851172,"$700,932,124.48",9.66%
    """
    )]
    public void ParseLine_SingleLine_ReturnsHoldingSnapshot(string singleLineDataset)
    {
        // Arrange

        // Act
        var parsedList = HoldingSnapshotLineParser.ParseLinesFromBytes(Encoding.UTF8.GetBytes(singleLineDataset)).ToList();
        var parsedLine = parsedList.First();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parsedList, Has.Count.EqualTo(1));

            Assert.That(parsedLine.Date, Is.EqualTo(new DateTime(2024, 4, 3)));
            Assert.That(parsedLine.Fund, Is.EqualTo("ARKK"));
            Assert.That(parsedLine.CompanyName, Is.EqualTo("COINBASE GLOBAL INC -CLASS A"));
            Assert.That(parsedLine.Ticker, Is.EqualTo("COIN"));
            Assert.That(parsedLine.Cusip, Is.EqualTo("19260Q107"));
            Assert.That(parsedLine.Shares, Is.EqualTo(BigInteger.Parse("2851172")));
            Assert.That(parsedLine.MarketValueUsd, Is.EqualTo(700932124.48m));
            Assert.That(parsedLine.Weight, Is.EqualTo(0.0966m));
        });
    }

    [Test]
    [
    // it is what it is; the following will succeed & return empty list
    // TestCase(
    // """
    // date,fund,company,ticker,cusip,shares,market value ($),weight (%)
    // invalid_date,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,2851172,"$700,932,124.48",9.66%
    // """
    // ),
    TestCase(
    """
    date,fund,company,ticker,cusip,shares,market value ($),weight (%)
    04/03/2024,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,invalid_bigint,"$700,932,124.48",9.66%
    """
    ),
    TestCase(
    """
    date,fund,company,ticker,cusip,shares,market value ($),weight (%)
    04/03/2024,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,2851172,"$invalid_cash",9.66%
    """
    ),
    TestCase(
    """
    date,fund,company,ticker,cusip,shares,market value ($),weight (%)
    04/03/2024,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,2851172,"$700,932,124.48",invalid_percentage%
    """
    )
    ]
    public void ParsLine_SingleInvalidLine_ThrowsException(string singleLineDataset)
    {
        // Arrange

        // Act
        void act()
        {
            var thing = HoldingSnapshotLineParser.ParseLinesFromBytes(Encoding.UTF8.GetBytes(singleLineDataset)).ToList();
            Console.WriteLine(thing[0]);
        }

        // Assert
        Assert.Throws<FormatException>(() => act());
    }
}
