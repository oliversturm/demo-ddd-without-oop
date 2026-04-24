using CsharpFp3.Library;

namespace CsharpFp3.Tests;

/// Tests covering the Result<T,E> type and its extension methods.
public class ResultTests
{
    // --- Ok / Fail construction ---

    [Fact]
    public void Ok_is_success()
    {
        var result = Result<int, string>.Ok(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Fail_is_failure()
    {
        var result = Result<int, string>.Fail("oops");

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Ok_exposes_value()
    {
        var result = Result<int, string>.Ok(42);

        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Fail_exposes_error()
    {
        var result = Result<int, string>.Fail("oops");

        Assert.Equal("oops", result.Error);
    }

    [Fact]
    public void Accessing_Value_on_a_failed_result_throws()
    {
        var result = Result<int, string>.Fail("oops");

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Accessing_Error_on_a_successful_result_throws()
    {
        var result = Result<int, string>.Ok(42);

        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    // --- Match ---

    [Fact]
    public void Match_calls_onSuccess_for_Ok()
    {
        var result = Result<int, string>.Ok(10);

        var output = result.Match(v => $"value:{v}", e => $"error:{e}");

        Assert.Equal("value:10", output);
    }

    [Fact]
    public void Match_calls_onFailure_for_Fail()
    {
        var result = Result<int, string>.Fail("bad");

        var output = result.Match(v => $"value:{v}", e => $"error:{e}");

        Assert.Equal("error:bad", output);
    }

    // --- Switch ---

    [Fact]
    public void Switch_calls_onSuccess_for_Ok()
    {
        var result = Result<int, string>.Ok(7);
        var called = false;

        result.Switch(_ => called = true, _ => { });

        Assert.True(called);
    }

    [Fact]
    public void Switch_calls_onFailure_for_Fail()
    {
        var result = Result<int, string>.Fail("err");
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
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public void Catch_returns_Fail_when_an_exception_is_thrown()
    {
        var result = Result<int, string>.Catch(
            () => throw new InvalidOperationException("boom"),
            ex => ex.Message
        );

        Assert.True(result.IsFailure);
        Assert.Equal("boom", result.Error);
    }

    // --- Bind ---

    [Fact]
    public void Bind_chains_to_the_next_result_on_success()
    {
        var result = Result<int, string>.Ok(5).Bind(v => Result<string, string>.Ok($"got {v}"));

        Assert.Equal("got 5", result.Value);
    }

    [Fact]
    public void Bind_short_circuits_on_failure_without_calling_the_binder()
    {
        var binderCalled = false;
        var result = Result<int, string>
            .Fail("nope")
            .Bind(v =>
            {
                binderCalled = true;
                return Result<string, string>.Ok($"got {v}");
            });

        Assert.False(binderCalled);
        Assert.Equal("nope", result.Error);
    }

    [Fact]
    public void Bind_propagates_the_inner_failure_when_the_binder_returns_Fail()
    {
        var result = Result<int, string>.Ok(5).Bind(_ => Result<string, string>.Fail("inner fail"));

        Assert.Equal("inner fail", result.Error);
    }

    // --- Map ---

    [Fact]
    public void Map_transforms_the_value_on_success()
    {
        var result = Result<int, string>.Ok(3).Map(v => v * 2);

        Assert.Equal(6, result.Value);
    }

    [Fact]
    public void Map_does_not_call_the_mapper_on_failure()
    {
        var mapperCalled = false;
        var result = Result<int, string>.Fail("err").Map(v =>
        {
            mapperCalled = true;
            return v * 2;
        });

        Assert.False(mapperCalled);
        Assert.Equal("err", result.Error);
    }

    // --- Tap ---

    [Fact]
    public void Tap_calls_the_action_and_returns_the_original_result_on_success()
    {
        var seen = -1;
        var result = Result<int, string>.Ok(8).Tap(v => seen = v);

        Assert.Equal(8, seen);
        Assert.Equal(8, result.Value);
    }

    [Fact]
    public void Tap_does_not_call_the_action_on_failure()
    {
        var called = false;
        Result<int, string>.Fail("err").Tap(_ => called = true);

        Assert.False(called);
    }

    // --- TapError ---

    [Fact]
    public void TapError_calls_the_action_and_returns_the_original_result_on_failure()
    {
        var seen = "";
        var result = Result<int, string>.Fail("bad").TapError(e => seen = e);

        Assert.Equal("bad", seen);
        Assert.Equal("bad", result.Error);
    }

    [Fact]
    public void TapError_does_not_call_the_action_on_success()
    {
        var called = false;
        Result<int, string>.Ok(1).TapError(_ => called = true);

        Assert.False(called);
    }

    // --- MapError ---

    [Fact]
    public void MapError_transforms_the_error_on_failure()
    {
        var result = Result<int, string>.Fail("oops").MapError(e => e.Length);

        Assert.Equal(4, result.Error);
    }

    [Fact]
    public void MapError_does_not_call_the_mapper_on_success()
    {
        var mapperCalled = false;
        var result = Result<int, string>.Ok(1).MapError(e =>
        {
            mapperCalled = true;
            return e.Length;
        });

        Assert.False(mapperCalled);
        Assert.Equal(1, result.Value);
    }

    // --- Select / SelectMany (LINQ support) ---

    [Fact]
    public void Select_is_equivalent_to_Map()
    {
        var result = Result<int, string>.Ok(4).Select(v => v + 1);

        Assert.Equal(5, result.Value);
    }

    [Fact]
    public void SelectMany_sequences_two_successful_results()
    {
        var result =
            from a in Result<int, string>.Ok(3)
            from b in Result<int, string>.Ok(4)
            select a + b;

        Assert.Equal(7, result.Value);
    }

    [Fact]
    public void SelectMany_short_circuits_when_the_first_result_fails()
    {
        var result =
            from a in Result<int, string>.Fail("first fail")
            from b in Result<int, string>.Ok(4)
            select a + b;

        Assert.Equal("first fail", result.Error);
    }

    [Fact]
    public void SelectMany_short_circuits_when_the_second_result_fails()
    {
        var result =
            from a in Result<int, string>.Ok(3)
            from b in Result<int, string>.Fail("second fail")
            select a + b;

        Assert.Equal("second fail", result.Error);
    }
}
