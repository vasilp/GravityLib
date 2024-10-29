using System.ComponentModel;
using GravityLib.Common.Extensions;
using Xunit;

namespace GravityLib.Common.Tests.Extensions;

public class EnumExtensionsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DescriptionOrderer_OrdersCorrecly(bool isAscOrder)
    {
        // Assign
        var items = new List<TheModel>()
        {
            new TheModel() { MySortableEnumProp = TestEnum.ABC },
            new TheModel() { MySortableEnumProp = TestEnum.DEF },
            new TheModel() { MySortableEnumProp = TestEnum.JHK },
        }
        .AsQueryable();

        var orderExpr = EnumExtensions.DescriptionOrderer((TheModel x) => x.MySortableEnumProp);

        // Act
        var orderedItems = isAscOrder
            ? items.OrderBy(orderExpr).ToList()
            : items.OrderByDescending(orderExpr).ToList();

        // Assert
        var isOrdered = true;
        for (var i = 0; i < orderedItems.Count - 1; i++)
        {
            var currentValue = orderedItems[i].MySortableEnumProp.GetDescription();
            var nextValue = orderedItems[i+1].MySortableEnumProp.GetDescription();

            var result = isAscOrder
                ? string.Compare(currentValue, nextValue)
                : string.Compare(nextValue, currentValue);

            if (result > 0)
            {
                isOrdered = false;
                break;
            }
        }

        Assert.True(isOrdered);
    }
}

internal class TheModel
{
    public TestEnum MySortableEnumProp { get; set; }
}

internal enum TestEnum
{
    [Description("3-ABC")]
    ABC,
    [Description("1-DEF")]
    DEF,
    [Description("2-JHK")]
    JHK,
}