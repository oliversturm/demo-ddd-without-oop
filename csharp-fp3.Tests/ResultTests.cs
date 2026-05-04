using CsharpFp3.Library;
using static CsharpFp3.Library.ResultModule;

namespace CsharpFp3.Tests;

/// Tests covering the Result<T,E> type and its extension methods.
public class ResultTests
{
    // --- Ok / Fail construction ---

    [Fact]
    public void Ok_is_success()
    {
        Result<int, string> result = new ResultOk<int, string>(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Fail_is_failure()
    {
        Result<int, string> result = new ResultFail<int, string>("oops");

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Ok_exposes_value_via_Match()
    {
        Result<int, string> result = new ResultOk<int, string>(42);

        Assert.Equal(42, result.Match(v => v, _ => 0));
    }

    [Fact]
    public void Fail_exposes_error_via_Match()
    {
        Result<int, string> result = new ResultFail<int, string>("oops");

        Assert.Equal("oops", result.Match(_ => "", e => e));
    }

    // --- Match ---

    [Fact]
    public void Match_calls_onSuccess_for_Ok()
    {
        Result<int, string> result = new ResultOk<int, string>(10);

        var output = result.Match(v => $"value:{v}", e => $"error:{e}");

        Assert.Equal("value:10", output);
    }

    [Fact]
    public void Match_calls_onFailure_for_Fail()
    {
        Result<int, string> result = new ResultFail<int, string>("bad");

        var output = result.Match(v => $"value:{v}", e => $"error:{e}");

        Assert.Equal("error:bad", output);
    }

    // --- Switch ---

    [Fact]
    public void Switch_calls_onSuccess_for_Ok()
    {
        Result<int, string> result = new ResultOk<int, string>(7);
        var called = false;

        result.Switch(_ => called = true, _ => { });

        Assert.True(called);
    }

    [Fact]
    public void Switch_calls_onFailure_for_Fail()
    {
        Result<int, string> result = new ResultFail<int, string>("err");
        var called = false;

        result.Switch(_ => { }, _ => called = true);

        Assert.True(called);
    }

    // --- Catch ---

    [Fact]
    public void Catch_returns_Ok_when_no_exception_is_thrown()
    {
        var result = Result<int, string>.Catch(() => 99, ex => ex.Message);

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Match(v => v, _ => 0));
    }

    [Fact]
    public void Catch_returns_Fail_when_an_exception_is_thrown()
    {
        var result = Result<int, string>.Catch(
            () => throw new InvalidOperationException("boom"),
            ex => ex.Message
        );

        Assert.True(result.IsFailure);
        Assert.Equal("boom", result.Match(_ => "", e => e));
    }

    // --- Bind ---

    [Fact]
    public void Bind_chains_to_the_next_result_on_success()
    {
        Result<int, string> result = new ResultOk<int, string>(5);
        var chained = result.Bind(v => new ResultOk<string, string>($"got {v}"));

        Assert.Equal("got 5", chained.Match(v => v, _ => ""));
    }

    [Fact]
    public void Bind_short_circuits_on_failure_without_calling_the_binder()
    {
        var binderCalled = false;
        Result<int, string> result = new ResultFail<int, string>("nope");
        var chained = result.Bind(v =>
        {
            binderCalled = true;
            return new ResultOk<string, string>($"got {v}");
        });

        Assert.False(binderCalled);
        Assert.Equal("nope", chained.Match(_ => "", e => e));
    }

    [Fact]
    public void Bind_propagates_the_inner_failure_when_the_binder_returns_Fail()
    {
        Result<int, string> result = new ResultOk<int, string>(5);
        var chained = result.Bind(_ => new ResultFail<string, string>("inner fail"));

        Assert.Equal("inner fail", chained.Match(_ => "", e => e));
    }

    // --- Map ---

    [Fact]
    public void Map_transforms_the_value_on_success()
    {
        Result<int, string> result = new ResultOk<int, string>(3);
        var mapped = result.Map(v => v * 2);

        Assert.Equal(6, mapped.Match(v => v, _ => 0));
    }

    [Fact]
    public void Map_does_not_call_the_mapper_on_failure()
    {
        var mapperCalled = false;
        Result<int, string> result = new ResultFail<int, string>("err");
        var mapped = result.Map(v =>
        {
            mapperCalled = true;
            return v * 2;
        });

        Assert.False(mapperCalled);
        Assert.Equal("err", mapped.Match(_ => "", e => e));
    }

    // --- Tap ---

    [Fact]
    public void Tap_calls_the_action_and_returns_the_original_result_on_success()
    {
        var seen = -1;
        Result<int, string> result = new ResultOk<int, string>(8);
        var tapped = result.Tap(v => seen = v);

        Assert.Equal(8, seen);
        Assert.Equal(8, tapped.Match(v => v, _ => 0));
    }

    [Fact]
    public void Tap_does_not_call_the_action_on_failure()
    {
        var called = false;
        Result<int, string> result = new ResultFail<int, string>("err");
        result.Tap(_ => called = true);

        Assert.False(called);
    }

    // --- TapError ---

    [Fact]
    public void TapError_calls_the_action_and_returns_the_original_result_on_failure()
    {
        var seen = "";
        Result<int, string> result = new ResultFail<int, string>("bad");
        var tapped = result.TapError(e => seen = e);

        Assert.Equal("bad", seen);
        Assert.Equal("bad", tapped.Match(_ => "", e => e));
    }

    [Fact]
    public void TapError_does_not_call_the_action_on_success()
    {
        var called = false;
        Result<int, string> result = new ResultOk<int, string>(1);
        result.TapError(_ => called = true);

        Assert.False(called);
    }

    // --- MapError ---

    [Fact]
    public void MapError_transforms_the_error_on_failure()
    {
        Result<int, string> result = new ResultFail<int, string>("oops");
        var mapped = result.MapError(e => e.Length);

        Assert.Equal(4, mapped.Match(_ => 0, e => e));
    }

    [Fact]
    public void MapError_does_not_call_the_mapper_on_success()
    {
        var mapperCalled = false;
        Result<int, string> result = new ResultOk<int, string>(1);
        var mapped = result.MapError(e =>
        {
            mapperCalled = true;
            return e.Length;
        });

        Assert.False(mapperCalled);
        Assert.Equal(1, mapped.Match(v => v, _ => 0));
    }

    // --- Select / SelectMany (LINQ support) ---

    [Fact]
    public void Select_is_equivalent_to_Map()
    {
        Result<int, string> result = new ResultOk<int, string>(4);
        var selected = result.Select(v => v + 1);

        Assert.Equal(5, selected.Match(v => v, _ => 0));
    }

    [Fact]
    public void SelectMany_sequences_two_successful_results()
    {
        Result<int, string> aResult = new ResultOk<int, string>(3);
        Result<int, string> bResult = new ResultOk<int, string>(4);
        var result =
            from a in aResult
            from b in bResult
            select a + b;

        Assert.Equal(7, result.Match(v => v, _ => 0));
    }

    [Fact]
    public void SelectMany_short_circuits_when_the_first_result_fails()
    {
        Result<int, string> aResult = new ResultFail<int, string>("first fail");
        Result<int, string> bResult = new ResultOk<int, string>(4);
        var result =
            from a in aResult
            from b in bResult
            select a + b;

        Assert.Equal("first fail", result.Match(_ => "", e => e));
    }

    [Fact]
    public void SelectMany_short_circuits_when_the_second_result_fails()
    {
        Result<int, string> aResult = new ResultOk<int, string>(3);
        Result<int, string> bResult = new ResultFail<int, string>("second fail");
        var result =
            from a in aResult
            from b in bResult
            select a + b;

        Assert.Equal("second fail", result.Match(_ => "", e => e));
    }
}
