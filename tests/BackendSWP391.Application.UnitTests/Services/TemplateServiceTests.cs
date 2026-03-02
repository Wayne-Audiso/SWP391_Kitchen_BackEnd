using System.Collections.Generic;
using FluentAssertions;
using BackendSWP391.Application.Services.Impl;
using Xunit;

namespace BackendSWP391.Application.UnitTests.Services;

public class TemplateServiceTests
{
    private readonly TemplateService _sut = new();

    [Fact]
    public void ReplaceInTemplate_Should_Replace_All_Words_From_Input()
    {
        // Arrange
        const string input = "Hello {Username}. Welcome!";
        var replaceWords = new Dictionary<string, string>
        {
            { "{Username}", "John983" }
        };

        // Act
        var result = _sut.ReplaceInTemplate(input, replaceWords);

        // Assert
        result.Should().Be("Hello John983. Welcome!");
    }
}

