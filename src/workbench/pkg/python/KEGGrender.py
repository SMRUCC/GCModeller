#!/usr/bin/python3

import json 
import os
import sys

from pathlib import Path
import random
import hashlib

try:
    import pandas as pd
except:
    # do nothing
    print("enrich_df parameter should be a file path.")

def render(enrich_df, 
           outputdir = "./", 
           map_id = "KEGG",
           pathway_links = "pathway_links",
           image = "dotnet:gcmodeller_20230401",
           kegg_maps = "/opt/biodeep/kegg/KEGG_maps.pack",
           run_debug = False):
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

    outputdir = os.path.abspath(outputdir)
    enrich_file = os.path.abspath(enrich_file)
    args = {
        "map_id": map_id,
        "pathway_links": pathway_links,
        "outputdir": outputdir,
        "enrich": enrich_file,
        "kegg_maps": kegg_maps
    }

    # setup the r_lambda environment variables
    setup_renvironment(args, outputdir)

    RSCRIPT_HOST   = "/usr/local/bin/Rscript.dll"    
    RSCRIPT_LAMBDA = "--lambda gcmodeller::KEGG_MapRender --SetDllDirectory /usr/local/bin/"

    # run docker pipeline shell command
    exit_code = run_docker(
        image_id = image, 
        app = RSCRIPT_HOST, 
        arguments = RSCRIPT_LAMBDA, 
        argv = args, 
        run_debug = run_debug
    )

    return exit_code

def setup_renvironment(argv, outputdir):

    print("*************** setup the R-dotnet runtime environment variables *****************")
    print("")
    print("view of your input parameters:")
    print(argv)

    # Serializing json and setup Rscript envrionment variables 
    # at current workspace
    argv_str = json.dumps(argv, indent = 2)
    pwd = os.path.abspath(outputdir)
    r_env = os.path.join(pwd, ".r_env")

    if not os.path.exists(r_env): 
        os.makedirs(r_env)

    jsonfile = open("{}/.r_env/run.json".format(pwd), 'w')
    jsonfile.write(argv_str)
    jsonfile.close()

def docker_shell(image_id, app, arguments, argv):
    docker = os.popen('which docker').read().strip()
    workspace = argv["outputdir"]

    run_pipeline = []
    run_pipeline.append("docker run -it --rm -e WINEDEBUG=-all")
    run_pipeline.append("-v /var/run/docker.sock:/run/docker.sock")
    run_pipeline.append("-v \"{0}:/bin/docker\"".format(docker))
    run_pipeline.append("-v \"{0}:{0}\"".format(argv["enrich"]))
    run_pipeline.append("-v \"{0}:{0}\"".format(workspace))
    run_pipeline.append("-v \"/tmp:/tmp\"")
    run_pipeline.append("-w \"{0}\"".format(workspace))
    run_pipeline.append("--privileged=true")
    run_pipeline.append(image_id)
    run_pipeline.append("dotnet")
    run_pipeline.append(app)
    
    return ' '.join(run_pipeline + arguments)

def run_docker(image_id, app, arguments, argv, run_debug = False):
    shell = 0
    shell_command = docker_shell(image_id, app, [arguments], argv)
    
    print("")
    print("Run shell commandline:")
    print(shell_command)
    print("")

    if not run_debug:
        shell = os.system(shell_command)
    else:
        print("skip of run docker shell command!")

    print("run docker pipeline job done!")
    print("exit={0}".format(shell))
    print("")

    return shell


if __name__ == "__main__":

    render("./associate_mt.xls", 
           outputdir = "./test_map/", 
           map_id = "KEGG",
           pathway_links = "pathway_links",
           image = "dotnet:gcmodeller_20230401",
           kegg_maps = "/opt/biodeep/kegg/KEGG_maps.pack",
           run_debug = False)