# Using the ``$ts`` static symbol

The ``$ts`` symbol is very similar to the jQuery ``$`` symbol. Through the ``$ts`` static symbol, you can access most of the web development api that comes with the ``Linq.js`` library.

## DOM operations

### 1. Query a node by Id / nodes by class

```ts
// get a single node by id
var node  = $ts("#xxxxx");
// query a node collection by class type
var nodes = $ts(".xxxxx");
```

> NOTE: The class name selector query will create a html node element collection: [DOMEnumerator](../Linq.ts/DOM/DOMEnumerator.ts). It is not recommended that query a node collection by class through ``$ts`` static function symbol in typescript, as you developing your web app under typescript, and the typed result of the ``$ts`` static function symbol is returns a extended node element by default, so it is not convenient for coding in typescript when query a set of node collection by ``$ts`` static function symbol. For more details information about DOM query in Linq.js library, please read the help document [**&lt;DOM query>**](./DOM/DOM_query.md)

Example:

```html
<div id="test"></div>
<script>
let node = $ts("#test");
</script>
```

### 2. Create a node by tag name

```ts
let node = $ts("<tagName>", arguments);
```

Example:

```html
<div id="test"></div>
<script>
let node = $ts("#test");
let link = $ts("<a>", {
    class: "link",
    id: "download",
    href: "javascript:void(0);",
    onclick: function() {
        alert("Hello world!");
    }
}).display("Hello world");

node.display(link);
</script>
```

Will generate a new ``<a>`` tag node and add in div ``#test``:

```html
<!-- generated html -->
<div id="test">
<a class="link" id="download" href="javascript:void(0);">Hello world</a>
</div>
```

### 3. Query nodes by css selector

The selector query function will create a html node element collection: [DOMEnumerator](../Linq.ts/DOM/DOMEnumerator.ts).

```ts
let nodes = $ts.select("css-selector"); 
```