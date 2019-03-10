## Using the ``$ts`` object

#### 1. Query a node by Id

```ts
var node = $ts("#xxxxx");
```

#### 2. Create a node by tag name

```ts
var node = $ts("<tagName>");
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