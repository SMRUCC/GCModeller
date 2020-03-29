# Extended Html Element

All of the html element that query by id or created from the ``$ts`` static symbol function is an extended html node element. For view the element interface of this extended html node element, please read this source file: [HTMLExtensions](https://github.com/biocad-cloud/data.ts/blob/master/Linq.ts/DOM/Extensions/Abstract.ts)

## Extended Chainning

There is a great programming feature in .NET framework is the extension method, by tagged a ``<Extension>`` custom attribute onto the function declaration, then you can make a pipeline code chaining in VisualBasic.NET programming. Although there is no such ``<Extension>`` attribute in typescript programming, but the Linq.js library try to provides the simulation of this pipeline chainning style programming feature in html node operator.

As we've mention above, by invoke the ``$ts`` static symbol function, you will get a extended html element node after get element by id or create new node operation, so that here is some operation that you can pipeline your code in a caller chaining:

```ts
// All of the function that listed below returns the node element itself
// So that you can continute reference to the same node in the next function call
// to create a pipeline style like chaining code
IHTMLElement.display(content)
IHTMLElement.clear()
IHTMLElement.append(node)
IHTMLElement.attr(name, value)
IHTMLElement.addClass(className)
IHTMLElement.removeClass(className)
IHTMLElement.css(stylesheet)
IHTMLElement.show()
IHTMLElement.hide()
```

As the example function that show above returns the node itself to the caller, so that we could implements such example code chaining like:

```ts
$ts("#data-table-div")
    .clear()
    .addClass("active")
    .display("<p>display content!</p>")
    .append("<img src='image.png'>")
    .css("width: 100%")
    .show();
```

The demo code show above will update such html content in your page:

```html
<div id="data-table-div" class="active" style="width: 100%; display:block;">
<p>display content!</p>
<img src='image.png'>
</div>
```
