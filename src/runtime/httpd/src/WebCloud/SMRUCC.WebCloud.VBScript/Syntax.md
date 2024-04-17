### 1. Include partial template

```html
<%= path/to/partial.vbhtml %>
```

### 2. Set variable value

```html
<!-- head.vbhtml -->
<title>$title</title> 

<!-- index.vbhtml -->
<?vb $title = "This is page title" ?>

<!-- will generates output for index.vbhtml -->
<title>This is page title</title> 
```

### 3. Using string resource

String resource xml:

```xml
<!-- Example: includes/strings.XML -->
<?xml version="1.0"?>
<vbhtml>
	<string name="KEGG">KEGG Reference Database</string>
	<string name="Uniprot">Uniprot Protein Reference database</string>
	<string name="RegPrecise">RegPrecise Motif Reference database</string>
	<string name="COG_myva">COG myva annotation tools</string>
</vbhtml>
```

Using string resource in vbhtml:

```
<a href="./KEGG.vbhtml"><%= @KEGG %></a>

<!-- will generates output -->
<a href="./KEGG.vbhtml">KEGG Reference Database</a>
```

### 4. For each loop

```html
<!-- Article post list "article.vbhtml" for each loop -->
<!--
    Will iterates all of the collection member element in the $post variable 
	and write property value to the template ``../includes/article.vbhtml``
-->
<?vb For $post As <%= ../includes/article.vbhtml %> ?>

<!-- Or --> 
<?vb 
	For $post As <%= ../includes/article.vbhtml %> 
?>
```