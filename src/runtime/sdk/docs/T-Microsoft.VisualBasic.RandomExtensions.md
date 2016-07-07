---
title: RandomExtensions
---

# RandomExtensions
_namespace: [Microsoft.VisualBasic](N-Microsoft.VisualBasic.html)_

Some extension methods for @"T:System.Random" for creating a few more kinds of random stuff.

> Imports from https://github.com/rvs76/superbest-random.git 


### Methods

#### NextBoolean
```csharp
Microsoft.VisualBasic.RandomExtensions.NextBoolean(System.Random)
```
Equally likely to return true or false. Uses @"M:System.Random.Next".

#### NextGaussian
```csharp
Microsoft.VisualBasic.RandomExtensions.NextGaussian(System.Random,System.Double,System.Double)
```
Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.

|Parameter Name|Remarks|
|--------------|-------|
|r|-|
|mu|Mean of the distribution|
|sigma|Standard deviation|


#### NextTriangular
```csharp
Microsoft.VisualBasic.RandomExtensions.NextTriangular(System.Random,System.Double,System.Double,System.Double)
```
Generates values from a triangular distribution.

|Parameter Name|Remarks|
|--------------|-------|
|r|-|
|a|Minimum|
|b|Maximum|
|c|Mode (most frequent value)|

> 
>  See http://en.wikipedia.org/wiki/Triangular_distribution for a description of the triangular probability distribution and the algorithm for generating one.
>  

#### Permutation
```csharp
Microsoft.VisualBasic.RandomExtensions.Permutation(System.Random,System.Int32,System.Int32)
```
Returns n unique random numbers in the range [1, n], inclusive. 
 This is equivalent to getting the first n numbers of some random permutation of the sequential numbers from 1 to max. 
 Runs in O(k^2) time.

|Parameter Name|Remarks|
|--------------|-------|
|rand|-|
|n|Maximum number possible.|
|k|How many numbers to return.|


#### Shuffle
```csharp
Microsoft.VisualBasic.RandomExtensions.Shuffle(System.Random,System.Collections.IList@)
```
Shuffles a list in O(n) time by using the Fisher-Yates/Knuth algorithm.

|Parameter Name|Remarks|
|--------------|-------|
|r|-|
|list|-|


#### Shuffle``1
```csharp
Microsoft.VisualBasic.RandomExtensions.Shuffle``1(System.Random,Microsoft.VisualBasic.List{``0}@)
```
Shuffles a list in O(n) time by using the Fisher-Yates/Knuth algorithm.

|Parameter Name|Remarks|
|--------------|-------|
|r|-|
|list|-|



