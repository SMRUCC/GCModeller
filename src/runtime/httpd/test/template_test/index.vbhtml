<%= includes/head.vbhtml %>

<!-- index.vbhtml -->
<% @title = "this is title value assign to 'includes/head.vbhtml'" %>

<p>
    @person.name, and age is @person.age.
</p>


<% @list = [
	{"name": "aaa", "score": 99},
	{"name": "bbb", "score": 89},
	{"name": "ccc", "score": 100},
] %>

<ul>
	<foreach @list>
		<li>@list.name(score: @list.score)</li>
	</foreach>
</ul>

<%= includes/footer.vbhtml %>