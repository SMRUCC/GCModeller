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

def dataframe_csv(enrich_df):
    enrich_file = "df_{}".format(random.randint(1000000, 10000000))
    enrich_file = "/tmp/{}.csv".format(enrich_file)

    if type(enrich_df) == str:
        enrich_file = enrich_df
    else:
        enrich_df.to_csv(enrich_file)

    return enrich_file

# const union_render = function(union_data, outputdir = "./",
#     id         = "KEGG", 
#     compound   = "compound", 
#     gene       = "gene", 
#     protein    = "protein", 
#     text.color = "white",
#     kegg_maps  = NULL) {

def union_render(enrich_df, outputdir = "./",
                 id         = "KEGG", 
                 compound   = "Compounds_KO", 
                 gene       = "Genes_KO", 
                 protein    = "Proteins_KO", 
                 text_color = "white",
                 kegg_maps  = None,
                 image      = "dotnet:gcmodeller_20240126",
                 run_debug  = False):

    enrich_file = dataframe_csv(enrich_df)
    v = []

    if not kegg_maps:
        v = ["union_data","outputdir","tmp"]
    else:        
        v = ["union_data","kegg_maps","outputdir","tmp"]
        kegg_maps = os.path.abspath(kegg_maps)

    outputdir = os.path.abspath(outputdir)
    enrich_file = os.path.abspath(enrich_file)
    args = {
        "union_data": enrich_file,
        "outputdir": outputdir,       
        "id": id, 
        "compound": compound, 
        "gene": gene, 
        "protein": protein, 
        "text.color": text_color,
        "kegg_maps": kegg_maps,
        "tmp": "/tmp/"
    }

    return r_lambda.call_lambda(
        "GCModeller::union_render",
        argv=args,
        options={"cache.enable": True},
        workdir=outputdir,
        docker=docker_image(id=image, volumn=v, name=None, tty=False),
        run_debug=run_debug
    )


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

    enrich_file = dataframe_csv(enrich_df)
    v = []

    if not kegg_maps:
        v = ["enrich","outputdir","tmp"]
    else:        
        v = ["enrich","kegg_maps","outputdir","tmp"]
        kegg_maps = os.path.abspath(kegg_maps)

    outputdir = os.path.abspath(outputdir)
    enrich_file = os.path.abspath(enrich_file)
    args = {
        "enrich": enrich_file,
        "map_id": map_id,
        "pathway_links": pathway_links,
        "outputdir": outputdir,       
        "min_objects": min_objects, 
        "kegg_maps": kegg_maps,
        "tmp": "/tmp/"
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

    render("./kegg_enrich_result.txt", 
            outputdir = "./test_map/", 
            map_id = "pathway_id",
            pathway_links = "links",
            image = "biodeepmsms:v5_20231002-patch4",
            kegg_maps = "./kegg_maps/",
            run_debug = False)