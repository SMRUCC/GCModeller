# VBHtml template syntax

## 1. variable render

```html
<p>@variable</p>
```

the variable could be just a scalar primitive value, example as string or number, andalso could be a clr object that may contains property.
if no propperty reference, then the ``ToString`` function will be used for variable rendering. for use a property value for the rendering,
just do as the vb code it does:

```html
<p>@obj.property_name.name2.name3</p>
```

the property name is case in-sensistive

## 2. set variable value

the variable value could be assigned from the clr function argument when do rendering of the vbhtml template, and also could be assigned from 
the vbhtml template file, this is usefull when do template rendering when the template file consists of multiple template file fragments, example
as:

```html
<!- includes/head.vbhtml ->
<title>@title</title>

<!- index.vbhtml ->
<% @title = "this is title value assign to 'includes/head.vbhtml'" %>

<head>
	<%= includes/head.vbhtml %>
</head>
```

so the template that show above will be rendering as this result html file:

```html
<head>
	<title>this is title value assign to 'includes/head.vbhtml'</title>
</head>
```

> note: the variable value is a json literal. and the symbol value has a lower priority when compares to the variable comes from the clr function calls.

## 3. string resource reference

the string resource file is useful for a template page in multiple language, example as rendering the html page in english and chinese language:

```html
<!- index.vbhtml ->

<%= index.@lang.json %>
<p>@hello</p>
```

and the string resource file:

```json
// index.zh.json 
{"hello": "你好!"}
// index.en.json
{"hello": "Hello!"}
```

when the variable ``lang`` its value is ``zh``, then the html result rendering will be:

```html
<!- index.vbhtml ->

<p>你好!</p>
```

otherwise for ``en``:

```html
<!- index.vbhtml ->

<p>Hello!</p>
```

## 4. include partial template

```html
<%= path/to/partial.vbhtml %>
```

## 5. for each loop

if the variable is a collection type, then you could do for each loop on the html template rendering, example as we have a variable:

```json
// list
[
	{"name": "aaa", "score": 99},
	{"name": "bbb", "score": 89},
	{"name": "ccc", "score": 100},
]
```

then we could use the template code for rendering such collection:

```vbhtml
<ul>
	<foreach @list>
		<li>@list.name(score: @list.score)</li>
	</foreach>
</ul>
```

will rendering as the result html file:

```html
<ul>
	<li>aaa(score: 99)</li>
	<li>bbb(score: 89)</li>
	<li>ccc(score: 100)</li>
</ul>
```
