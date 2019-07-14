## Using the ``$ts`` object

#### 1. Query a node by Id / nodes by class

```ts
// a single node
var node  = $ts("#xxxxx");
// a node collection
var nodes = $ts(".xxxxx");
```

> NOTE: The class name selector query will create a html node element collection: [DOMEnumerator](../Linq.ts/DOM/DOMEnumerator.ts).

Example:

```html
<div id="test"></div>
<script>
var node = $ts("#test");
</script>
```

#### 2. Create a node by tag name

```ts
var node = $ts("<tagName>", arguments);
```

Example:

```html
<div id="test"></div>
<script>
var node = $ts("#test");
var link = $ts("<a>", {
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
<!-- Generated html -->
<div id="test">
<a class="link" id="download" href="javascript:void(0);">Hello world</a>
</div>
```

#### 3. Query nodes by css selector

The selector query function will create a html node element collection: [DOMEnumerator](../Linq.ts/DOM/DOMEnumerator.ts).

```ts
var nodes = $ts.select("css-selector"); 
```

## Namespace and Modules

#### 1. Framework Global objects

1. [Global.ts](../Linq.ts/Global.ts): Export global function and global objects from this framework.
2. [Type.ts](../Linq.ts/Type.ts): Get object type information, not working for object type: ``{...}``

#### 2. DOM helper

[DOM](../Linq.ts/DOM/)

#### 3. Collections

[Collections](../Linq.ts/Collections/)

#### 4. csv data frame helper

[csv](../Linq.ts/csv/)