/*
       Class: Graph.Node
  
       A <Graph> node.
  
       Implements:
  
       <Accessors> methods.
  
       The following <Graph.Util> methods are implemented by <Graph.Node>
  
      - <Graph.Util.eachEdge>
      - <Graph.Util.eachLevel>
      - <Graph.Util.eachSubgraph>
      - <Graph.Util.eachSubnode>
      - <Graph.Util.anySubnode>
      - <Graph.Util.getSubnodes>
      - <Graph.Util.getParents>
      - <Graph.Util.isDescendantOf>
  */
class Node {
    constructor(opt) {
        var innerOptions = {
            'id': '',
            'name': '',
            'data': {},
            'adjacencies': {}
        };
        extend(this, merge(innerOptions, opt));
    };

    fromJSON(json) {
        return new Graph.Node(json);
    };


    toJSON() {
        return {
            id: this.id,
            name: this.name,
            data: this.serializeData(this.data)
        };
    }

    serializeData(data) {
        var serializedData = {},
            parents = data.parents,
            parentsCopy, i, l;

        if (parents) {
            parentsCopy = Array(parents.length);
            for (i = 0, l = parents.length; i < l; ++i) {
                parentsCopy[i] = parents[i].toJSON();
            }
        }

        for (i in data) {
            serializedData[i] = data[i];
        }

        delete serializedData.parents;
        delete serializedData.bundle;
        serializedData = JSON.parse(JSON.stringify(serializedData));

        if (parentsCopy) {
            serializedData.parents = parentsCopy;
        }

        return serializedData;
    }

    /*
       Method: adjacentTo
 
       Indicates if the node is adjacent to the node specified by id
 
       Parameters:
 
          id - (string) A node id.
 
       Example:
       (start code js)
        node.adjacentTo('nodeId') == true;
       (end code)
    */
    adjacentTo(node) {
        return node.id in this.adjacencies;
    }

    /*
       Method: getAdjacency
 
       Returns a <Graph.Edge> object connecting the current <Graph.Node> and the node having *id* as id.
 
       Parameters:
 
          id - (string) A node id.
    */
    getEdge(id) {
        return this.adjacencies[id];
    }

    /*
       Method: toString
 
       Returns a String with information on the Node.
 
    */
    toString() {
        return 'Node(' + JSON.stringify([this.id, this.name, this.data, this.adjacencies]) + ')';
    }
};