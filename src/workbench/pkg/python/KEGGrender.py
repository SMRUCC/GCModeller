#!/usr/bin/python3

import os
import r_lambda
from r_lambda.docker import docker_image

# pip3 install r_lambda

from pathlib import Path
import random
import hashlib

try:
    import pandas as pd
except:
    # do nothing
    print("enrich_df parameter should be a file path.")

# const KEGG_MapRender = function(enrich, 
#    map_id        = "KEGG",
#    pathway_links = "pathway_links",
#    outputdir     = "./",
#    min_objects   = 0,
#    kegg_maps     = NULL) {

def render(enrich_df, 
           outputdir     = "./", 
           map_id        = "KEGG",
           pathway_links = "pathway_links",           
           min_objects   = 0,
           kegg_maps     = None,
           image         = "dotnet:gcmodeller_20230401",
           run_debug     = False):
    """
    Do KEGG pathway map rendering locally
    ----
    enrich_df: a pandas dataframe object that should contains at least these data fields:
        1. KEGG: the kegg pathway map id
        2. pathway_links: the kegg pathway map url for do kegg map image rendering

        or else this parameter value could be a file path to the target kegg enrichment 
        result table file.

    image: the gcmodeller docker image id
    outputdir: the directory path for output the kegg pathway map rendering result
    pathway_links: the data field name for get the kegg pathway map link url, default is 'pathway_links'
    map_id: the data field name for get the kegg pathway map id, default is 'KEGG'
    """

    enrich_file = "df_{}".format(random.randint(1000000, 10000000))
    enrich_file = "/tmp/{}.csv".format(enrich_file)

    if type(enrich_df) == str:
        enrich_file = enrich_df
    else:
        enrich_df.to_csv(enrich_file)

    v = []

    if not kegg_maps:
        v = ["enrich_file","outputdir"]
    else:
        v = ["enrich_file","kegg_maps","outputdir"]

    outputdir = os.path.abspath(outputdir)
    enrich_file = os.path.abspath(enrich_file)
    args = {
        "enrich": enrich_file,
        "map_id": map_id,
        "pathway_links": pathway_links,
        "outputdir": outputdir,       
        "min_objects" min_objects, 
        "kegg_maps": kegg_maps
    }

    return r_lambda.call_lambda(
        "GCModeller::KEGG_MapRender",
        argv=args,
        options={"cache.enable": True},
        workdir=outputdir,
        docker=docker_image(id=image, volumn=v, name=None, tty=False),
        run_debug=run_debug
    )

if __name__ == "__main__":

    render("./associate_mt.xls", 
           outputdir = "./test_map/", 
           map_id = "KEGG",
           pathway_links = "pathway_links",
           image = "dotnet:gcmodeller_20230401",
           kegg_maps = "/opt/biodeep/kegg/KEGG_maps.pack",
           run_debug = False)