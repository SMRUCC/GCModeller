function d3Network(jsonFile, width, height) {
	
    var color = d3.scale.category20();
    var force = d3.layout.force()
        .charge(-120)
        .linkDistance(30)
        .size([width, height]);

    var svg = d3.select("body").append("svg")
		.attr("fill", "#DBF3FF")
        .attr("width", width)
        .attr("height", height)
		.attr("class", "chart");
    
    d3.json(jsonFile, function (error, graph) {
        if (error) throw error;

        force
            .nodes(graph.nodes)
            .links(graph.links)
            .start();

        var link = svg.selectAll(".link")
            .data(graph.links)
            .enter().append("line")
            .attr("class", "link")
            .style("stroke-width", 0.5);

        var node = svg.selectAll(".node")
            .data(graph.nodes)
            .enter().append("circle")
            .attr("class", "node")
            .attr("r", function (d) {
                return Math.sqrt(d.size)+1;
            })
            .style("fill", function (d) {
                return color(d.group);
            })
            .style("opacity", 0.8)
            .call(force.drag);

        node.append("title")
			.attr("class", "tooltip")
            .style("font-size", 16)
            .html(function (d) {
                return "name:\t" + d.name + "\ntype:\t" + d.type + "\nlinks:\t" + d.size;
            });

        force.on("tick", function () {
            link.attr("x1", function (d) { return d.source.x; })
                .attr("y1", function (d) { return d.source.y; })
                .attr("x2", function (d) { return d.target.x; })
                .attr("y2", function (d) { return d.target.y; });

            node.attr("cx", function (d) { return d.x; })
                .attr("cy", function (d) { return d.y; });
        });
    });
}