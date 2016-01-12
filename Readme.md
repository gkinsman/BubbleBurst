
I've made a few notes on the implementation of 


### Immutability

To me the most interesting part is the use of `System.Collections.Immutable.ImmutableList` to represent the game grid. ImmutableList is a persistent data structure, which means that it's possible to share memory between different instances.

The API for a simple ImmutableList looks like this:

```csharp
	var builder = ImmutableList.CreateBuilder<int>();

    builder.AddRange(new[] { 1, 2, 3, 4 });

    ImmutableList<int> foo = list.ToImmutable();
```

```csharp
 To then mutate the list to add further items:

    builder = foo.ToBuilder();

    builder.AddRange(new[] { 5, 6, 7, 8 });

    ImmutableList<int> bar = builder.ToImmutable();
```

Under the covers, both `immutable` and `newImmutable` are now sharing memory (the values `[1,2,3,4]`)! ImmutableList does this by implementing an AVL tree which makes mutations non-destructive and requires minimal data copying. [Stephen Engelhardt has a great explanation of how they work](http://stevenengelhardt.com/2015/01/13/exploring-the-net-corefx-part-13-immutablelist-is-an-avl-tree/).

#### But we need a 2D grid...

In order to implement a 2D grid, we need an `ImmutableList<ImmutableList<BubbleBurst.Game.Bubble>>`. Creating a builder for the outer list is easy - we just use the built in one) - but that doesn't allow us to mutate the inner collection, which are the lists actually storing the bubbles. To work around that, there's a wrapper around the outer list builder that provides an indexer for which the setter manages the mutation:

```csharp
public class Builder
{
    private readonly ImmutableList<ImmutableList<Bubble>>.Builder _gridBuilder;

    internal Builder(ImmutableList<ImmutableList<Bubble>>.Builder grid)
    {
        this._gridBuilder = grid;
    }

    public ImmutableBubbleBurstGrid ToImmutable()
    {
        return new ImmutableBubbleBurstGrid(_gridBuilder.ToImmutable());
    }

    public Bubble this[int col, int row]
    {
        set
        {
            var list = _gridBuilder[row].ToBuilder();
            list[col] = value;
            _gridBuilder[row] = list.ToImmutable();
        }
    }
}
```

This ensures we're still sharing memory and those builders are pretty much free to create and use. 

I should add that using ImmutableList is largely unnecessary since we're greatly reducing the search space with the selection strategies. It's primary responsibility is to maintain my interest in this project and to learn about the pattern.

###Algorithms

There's a couple of interesting algorithms in the game implementation itself, let's take a look at them!

#### Flood fill

A [flood fill](https://en.wikipedia.org/wiki/Flood_fill) is used to put all the bubbles into groups.  It works by iterating over each bubble of the grid, checking to see if it's part of a group already, and if not then recursively going north/south/east/west and adding any same-coloured bubbles to the current group.

It's also by far the most costly part of the program as it's a fairly naive implementation of a flood fill. As DotTrace shows, it's responsible for a good 74% of the program's runtime:

![](http://i.imgur.com/W2H213x.png)

There's certainly room for improvement here, but I'm not sure it would have as drastic an impact on results as using good heuristics. Ultimately, we still need to intelligently select which grids to test - doing it say, 50% faster might not change the results that much.

[https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/BubbleGroupFinder.cs#L23](https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/BubbleGroupFinder.cs#L23)


#### JumpTillTheresNoGaps

The `JumpTillTheresNoGaps` function is one of two algorithms responsible for resolving the grid state after popping a bubble group. It works by beginning at the bottom of each column, and works its way up. As it detects gaps it increments an adjustment value which when encountering a bubble, moves the bubble down by that amount.

[https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/Extensions/GridUtils.cs#L166](https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/Extensions/GridUtils.cs#L166)

#### PushColumnsRight

The `PushColumnsRight` function is fairly straightforward: for each column, check to see if the column is empty. If it is, then move from right to left and pull the bubbles to the right, using an adjustment similar to `JumpTillTheresNoGaps`.

[https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/Extensions/GridUtils.cs#L107](https://github.com/gkinsman/BubbleBurst/blob/master/BubbleBurst.Game/Extensions/GridUtils.cs#L107)

Together they perform quite well, though we're doing many fewer of them than the FindBubbleGroup function:

![](http://i.imgur.com/wVmJ0mA.png)
