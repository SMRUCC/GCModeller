# Microsoft.VisualBasic.Parallel
> Further reading: [Parallel library of GCModeller parallel computing](http://www.codeproject.com/Articles/1076209/Easy-Distribution-Computing-in-VisualBasic)


##### Steps of the distributed computing in this library

>
1. Create the instance of the TaskInvoke object on your server side program
2. Creates the function to analysis the data
3. Create a TaskHost object on your local client, and connect to the remote through IPEndPoint
4. Using AddressOf function to gets the function Delegate pointer of your target function which is want to running on the remote.
5. Calling this function Delegate pointer on the remote machine through TaskHost.Invoke and gets the returns result.

Through these steps, your function on the local client should be running on the cloud and you are able to integrated your cloud server calculation resource onto your local client app to provides the powerfull cloud computing feature for your application.

Important NOTE:
+ Please notices that the function which is running on the remote machine should be a statics method and donot reference to the module variable as the module variable is probably not initialized on the remote machine, just using the variable limits in your function inner local variable.
Example:

```vb.net
Module Test1
    Dim m_var As Integer

    ' As the m_var is a module variable, and this variable is probley is not initialized on the remote machine
    ' So that this function is always returns ZERO
    ' So that reference to the module variable is not recommended
    Public Function MayFailure As Integer
        Return Test1.m_var
    End Function

    ' As all of the variable can be initialized in the function body, 
    ' so that this function is running in the correctly state
    Public Function IsSuccess As Integr
     	Dim var As Integer
        Return var
    End Function
End Module
```

+ The object of the remote function pointer its parameter should be a simple class, which is the Class object instance can be serialization and deserialization by the json through the property, if the class is initialize by a method, and then this class is not a **"simple"** class, Json transfer of this class and create instance at the remote machine could be failure or running in a unexpected result;
+ Currently this library just support the statics method, but the instance method will be supported in the feature works;
+ If want to read and write the file on the remote machine, the RemoteFileStream is available for your remote function read local file to the remote machine and read remote file on the server to your client.


### Usage of the remote linq script to query remote resource

```vb.net
Imports <namespace>

var source = http://linq.gcmodeller.org/kegg/ssdb/nucl/xcb
var result = from x as <type> in $source let <statement> where <test_statement> select <ctor.statement>
```

#### RQL language

RQL is a repository query language with the linq expression syntax like feature. And this query engine is power by the Linq script library.
Unlike the SQL langauge, the RQL language is a object-oriented language for query the data source.


#####Using RQL language rest API
First the repository server should implements the RQL services on the server program, the RQL services which is available  in the RQL project;
Then your client user that can query your repository server directly by using linq script in two ways, examples as:
###### 1. query repository throw linq script

```vb.net
Imports GCModeller.RQL

var source = "http://linq.gcmodeller.org/kegg/ssdb"
var result = from prot as prot in $source where regex.match( prot-> title,"XC_").success select prot
```

The code show above that query all of the protein fasta sequence from the repository **http://linq.gcmodeller.org/kegg/ssdb**

###### 2. query repository throw rest API extension
And you also can query the server directly from the rest API.
For example, there is a RQL repository services implements on server
**http://linq.gcmodeller.org**

And then query that want to apply on the object type **KEGG/ssdb/prot**, then a rest API url is available as:
**http://linq.gcmodeller.org/kegg/ssdb/prot?where=regex.match($-> title,"XC_").success**

if the where argments is not applied on the rest API, then the entired of the repository resource will be returns, and this situation is not recommended as the network transfer resource is wasted for the non-uselessness datas.

The where expression statement language syntax can references to the **VisualBasic language** or **Shoal language**

Then a uid string will be returns from the server, and then you can execute the move next action to gets the remote linq iterator value from the
**http://linq.gcmodeller.org/moveNext?uid=&lt;uid_returns_from_previous_step>**

+ if the data source iterator is read done, then a error code 404 will be returns
+ if the uid is can not be found on the repository services, then a error code of 501 will be returns
+ if the data source is avaliable for iterator move next, then the object json text will be returns

