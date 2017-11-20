Imports System.ComponentModel

''' <summary>
''' Type of an individual module (pixel) of a QR symbol.
''' </summary>
Public Enum ModuleType
    Light
    Dark
End Enum

''' <summary>
''' QR encoding modes
''' </summary>
Public Enum Mode
    ECI = 0
    Numeric = 1
    AlphaNumeric = 2
    [Byte] = 3
    Kanji = 4
    StructuredAppend = 5
    FNC1_FirstPosition = 6
    FNC1_SecondPosition = 7
    Terminator = 8
End Enum

''' <summary>
''' QR symbol types
''' </summary>
Public Enum SymbolType
    Micro
    Normal
End Enum

''' <summary>
''' QR error correction modes
''' </summary>
Public Enum ErrorCorrection
    <Description("Error-Detection Only")>
    None = 0
    <Description("L (7%)")>
    L = 1
    <Description("M (15%)")>
    M = 2
    <Description("Q (25%)")>
    Q = 3
    <Description("H (30%)")>
    H = 4
End Enum
