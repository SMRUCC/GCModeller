# DOM query

The DOM query operation in Linq.js library is consist with 3 kinds of expression:

## Get by id and get by class

Get node by id operation in Linq.js library is based on the ``context.getElementById`` and get node collection by class is based on the ``document.getElementsByClassName`` method.

> The ``context`` object could be a html ``Document`` object or a specific ``HTMLElement`` for limits the value range.

We recommended that query a single node by id through ``$ts`` static symbol function directly and using the ``$ts.select`` symbol function for query node set by class name. Both of these two kind of html node element query, their query expression syntax is a valid css query expression as for query a node by id you should apply the ``#id`` expression and the class name query is ``.class``. But the Linq.js implements these two kind of node elemenet query operation in different way when compares with the css selector query. For implements a node query by id and node set query by class name, the Linq.js library will trim the first prefix ``#`` or ``.`` symbol, and the get the id or class name value, then apply to the ``context.getElementById`` or ``document.getElementsByClassName`` function.

For an instance example of these two query operation:

```ts
// get element by id
let id = $ts("#id");
// get node set by class name
let list = $ts.select(".list");
```

> All of the node element that query by id from the ``$ts`` static symbol function is a kind of extended node element, for more details information about the extended node element, please read this help document: [**&lt;Extended Html Element>**](./extended_node.md).

## Get by CSS selector

The css selector is works based on the ``context.querySelectorAll`` or the ``document.querySelector``, which accept the css query text and then given a set of node collection of a single node element.

> The ``context`` object could be a html ``Document`` object or a specific ``HTMLElement`` for limits the value range.

Apply a CSS node element selectin is keeps the same as select by class id, query through the ``$ts.select`` symbol function is recommended. Example as:

```ts
// selector will returns a collection of html link element with https url
let links: DOMEnumerator<HTMLAnchorElement> = $ts.select('a[src^="https"]');
```

### Query meta tag value

There is a kind of special css query in ``Linq.js`` framework: query the &lt;meta> tag value via expression syntax ``@tag_name``. For an instance example, there is a meta tag which is named ``app`` in your html file:

```html
<meta name="app" content="The_App_Name" />
```

Then you can get the string content value of the ``app`` meta tag with the query like:

```ts
let app: string = <any>$ts("@app");

// will print content string in your developer console: The_App_Name
console.log(app);
```

The query of the meta tag value is special when compares it with other css selector operation in Linq.js library, as it is recommended that query through ``$ts`` static symbol directory, not recommended from the ``$ts.select`` symbol function. The query expression result of the meta tag value is not a html element node but a single string value, and its query expression syntax is not a valid css query syntax.

For implements the meta tag value query in ``$ts`` static symbol function, the ``Linq.js`` library first trim the ``@`` character get the meta tag name attribute value, and then create a new css query expression in example as ``meta[name~="${name}"]``. So that the meta tag value query is still a css query operation in Linq.js library.

## Create new node

For create a new node by using Linq.js library, its is very easy that invoke the ``$ts`` static symbol function by passing the ``<tagName>`` as parameter value, an instance example as:

```ts
// The $ts function will create a new HTMLAnchorElement for your code
let a = $ts("<a>");
```

> All of the node element that created by the ``$ts`` static symbol function is a kind of extended node element, for more details information about the extended node element, please read this help document: [**&lt;Extended Html Element>**](./extended_node.md).

For add initialize attribute value or binding the events for the new node element, just using the query argument of the ``$ts`` static symbol function, an instance example as:

```ts
let a = $ts("<a>", {
    class: ["btn", "btn-blue"],
    type: "button",
    href: "javascript:void(0);",
    onclick: function () {
        // 在页面的meta信息中有project_id
        // 如果analysis_file参数为空，则上传新文件
        // 反之，编辑现有的分析文件
        window.open(`/project/file?analysis_file=${file.id}`, "_blank");
    }
});
```

Using Linq.js library for create a new html node element is very convenient when compare with the vanilla js as your must write this long code example in vanilla js:

```js
let a = document.createElement("a");

a.setAttribute("class", "btn btn-blue");
a.setAttribute("type", "button");
a.setAttribute("href", "javascript:void(0);");
a.onclick = function() {
    window.open(`/project/file?analysis_file=${file.id}`, "_blank");
};
```

For more details information about ``$ts`` query parameter, please view this source file: [TypeScriptArgument.ts](https://github.com/biocad-cloud/data.ts/blob/master/Linq.ts/Framework/Define/Abstracts/TypeScriptArgument.ts)