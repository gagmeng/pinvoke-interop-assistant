﻿' Copyright (c) Microsoft Corporation.  All rights reserved.
'The following code was generated by Microsoft Visual Studio 2005. 'The test owner should check each test for validity.
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports System.IO
Imports PInvoke.Parser
Imports Xunit

'''<summary>
'''This is a test class for PInvoke.Parser.Scanner and is intended
'''to contain all PInvoke.Parser.Scanner Unit Tests
'''</summary>
Public Class ScannerTest

    Private _defaultOpts As ScannerOptions
    Private _lineOpts As ScannerOptions
    Private _commentOpts As ScannerOptions
    Private _rudeEndOpts As ScannerOptions

    Public Sub New()
        _defaultOpts = New ScannerOptions()
        _defaultOpts.ThrowOnEndOfStream = False
        _defaultOpts.HideComments = True
        _defaultOpts.HideNewLines = True
        _defaultOpts.HideWhitespace = True

        _lineOpts = New ScannerOptions()
        _lineOpts.ThrowOnEndOfStream = False
        _lineOpts.HideComments = True
        _lineOpts.HideNewLines = False
        _lineOpts.HideWhitespace = True

        _commentOpts = New ScannerOptions()
        _commentOpts.HideComments = False
        _commentOpts.ThrowOnEndOfStream = False
        _commentOpts.HideNewLines = True
        _commentOpts.HideWhitespace = True

        _rudeEndOpts = New ScannerOptions()
        _rudeEndOpts.ThrowOnEndOfStream = True
    End Sub

    Private Sub VerifyNext(ByVal scanner As Scanner, ByVal tt As TokenType)
        Dim token As Token = scanner.GetNextToken()
        Assert.Equal(tt, token.TokenType)
    End Sub

    Private Sub VerifyNext(ByVal scanner As Scanner, ByVal tt As TokenType, ByVal val As String)
        Dim token As Token = scanner.GetNextToken()
        Assert.Equal(tt, token.TokenType)
        Assert.Equal(val, token.Value)
    End Sub

    Private Sub VerifyPeek(ByVal scanner As Scanner, ByVal tt As TokenType)
        Dim token As Token = scanner.PeekNextToken()
        Assert.Equal(tt, token.TokenType)
    End Sub

    Private Sub VerifyPeek(ByVal scanner As Scanner, ByVal tt As TokenType, ByVal val As String)
        Dim token As Token = scanner.PeekNextToken()
        Assert.Equal(tt, token.TokenType)
        Assert.Equal(val, token.Value)
    End Sub

    Private Function CreateScanner(ByVal data As String) As Scanner
        Return New Scanner(New StringReader(data))
    End Function

    Private Function CreateScanner(ByVal data As String, ByVal opts As ScannerOptions) As Scanner
        Return New Scanner(New TextReaderBag(New StringReader(data)), opts)
    End Function

    <Fact>
    Public Sub BasicScan1()
        Dim scanner As Scanner = CreateScanner("#define ", _defaultOpts)
        Dim token As Token = scanner.GetNextToken()
        Assert.Equal(TokenType.PoundDefine, token.TokenType)
    End Sub

    <Fact>
    Public Sub BasicScan2()
        Dim scanner As Scanner = CreateScanner("#if ", _defaultOpts)
        Dim token As Token = scanner.GetNextToken()
        Assert.Equal(TokenType.PoundIf, token.TokenType)
    End Sub

    <Fact>
    Public Sub BasicScan3()
        Dim scanner As Scanner = CreateScanner("{} ", _defaultOpts)
        VerifyNext(scanner, TokenType.BraceOpen)
        VerifyNext(scanner, TokenType.BraceClose)
    End Sub

    <Fact>
    Public Sub BasicScan4()
        Dim scanner As Scanner = CreateScanner("#define {", _defaultOpts)
        VerifyNext(scanner, TokenType.PoundDefine)
        VerifyNext(scanner, TokenType.BraceOpen)
    End Sub

    <Fact>
    Public Sub BasicScan5()
        Dim scanner As Scanner = CreateScanner("#define val {}", _defaultOpts)
        VerifyNext(scanner, TokenType.PoundDefine)
        VerifyNext(scanner, TokenType.Word, "val")
        VerifyNext(scanner, TokenType.BraceOpen)
        VerifyNext(scanner, TokenType.BraceClose)
    End Sub

    <Fact>
    Public Sub BasicScan6()
        Dim scanner As Scanner = CreateScanner("name[]")
        VerifyNext(scanner, TokenType.Word, "name")
        VerifyNext(scanner, TokenType.BracketOpen)
        VerifyNext(scanner, TokenType.BracketClose)
    End Sub

    <Fact>
    Public Sub BasicScan7()
        Dim scanner As Scanner = CreateScanner("foo[2]")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.BracketOpen)
        VerifyNext(scanner, TokenType.Number, "2")
        VerifyNext(scanner, TokenType.BracketClose)
    End Sub

    <Fact>
    Public Sub BasicScan8()
        Dim scanner As Scanner = CreateScanner("typedef")
        VerifyNext(scanner, TokenType.TypedefKeyword, "typedef")
    End Sub

    <Fact>
    Public Sub BasicScan9()
        Dim scanner As Scanner = CreateScanner("_hello")
        VerifyNext(scanner, TokenType.Word, "_hello")
    End Sub

    <Fact>
    Public Sub BasicScan10()
        Dim scanner As Scanner = CreateScanner("!foo!")
        VerifyNext(scanner, TokenType.Bang, "!")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.Bang, "!")
    End Sub

    <Fact>
    Public Sub BasicScan11()
        Dim scanner As Scanner = CreateScanner("+-/%")
        VerifyNext(scanner, TokenType.OpPlus, "+")
        VerifyNext(scanner, TokenType.OpMinus, "-")
        VerifyNext(scanner, TokenType.OpDivide, "/")
        VerifyNext(scanner, TokenType.OpModulus, "%")
    End Sub

    <Fact>
    Public Sub BasicScan12()
        Dim scanner As Scanner = CreateScanner("<=>=")
        VerifyNext(scanner, TokenType.OpLessThanOrEqual, "<=")
        VerifyNext(scanner, TokenType.OpGreaterThanOrEqual, ">=")
    End Sub

    <Fact>
    Public Sub BasicScan13()
        Dim scanner As Scanner = CreateScanner("0x4 0x5 0xf")
        VerifyNext(scanner, TokenType.HexNumber, "0x4")
        VerifyNext(scanner, TokenType.WhiteSpace, " ")
        VerifyNext(scanner, TokenType.HexNumber, "0x5")
        VerifyNext(scanner, TokenType.WhiteSpace, " ")
        VerifyNext(scanner, TokenType.HexNumber, "0xf")
    End Sub

    <Fact>
    Public Sub BasicScan14()
        Dim scanner As Scanner = CreateScanner("__declspec(""foo"")")
        VerifyNext(scanner, TokenType.DeclSpec)
        VerifyNext(scanner, TokenType.ParenOpen)
        VerifyNext(scanner, TokenType.QuotedStringAnsi)
        VerifyNext(scanner, TokenType.ParenClose)
    End Sub

    <Fact>
    Public Sub BasicScan15()
        Dim scanner As Scanner = CreateScanner("a>>1")
        VerifyNext(scanner, TokenType.Word)
        VerifyNext(scanner, TokenType.OpShiftRight)
        VerifyNext(scanner, TokenType.Number)
    End Sub

    <Fact>
    Public Sub BasicScan16()
        Dim scanner As Scanner = CreateScanner("a<<1")
        VerifyNext(scanner, TokenType.Word)
        VerifyNext(scanner, TokenType.OpShiftLeft)
        VerifyNext(scanner, TokenType.Number)
    End Sub

    <Fact>
    Public Sub BasicScan17()
        Dim scanner As Scanner = CreateScanner("$foo")
        VerifyNext(scanner, TokenType.Text)
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    <Fact>
    Public Sub BasicScan18()
        Dim scanner As Scanner = CreateScanner("__$foo")
        VerifyNext(scanner, TokenType.Word, "__$foo")
    End Sub

    <Fact>
    Public Sub BasicScan19()
        Dim scanner As Scanner = CreateScanner("__inline inline")
        VerifyNext(scanner, TokenType.InlineKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.InlineKeyword)
    End Sub

    <Fact>
    Public Sub BasicScan20()
        Dim scanner As Scanner = CreateScanner("volatile __clrcall 5")
        VerifyNext(scanner, TokenType.VolatileKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.ClrCallKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Number, "5")
    End Sub

    <Fact>
    Public Sub BasicScan21()
        Dim scanner As Scanner = CreateScanner("__ptr32 __ptr64")
        VerifyNext(scanner, TokenType.Pointer32Keyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Pointer64Keyword)
    End Sub

    <Fact>
    Public Sub BasicScan22()
        Dim scanner As Scanner = CreateScanner("-one")
        VerifyNext(scanner, TokenType.OpMinus, "-")
        VerifyNext(scanner, TokenType.Word, "one")
    End Sub

    <Fact>
    Public Sub BasicScan23()
        Dim scanner As Scanner = CreateScanner("-22one")
        VerifyNext(scanner, TokenType.OpMinus, "-")
        VerifyNext(scanner, TokenType.Word, "22one")
    End Sub

    <Fact>
    Public Sub BasicScan24()
        Dim scanner As Scanner = CreateScanner("'f' 'a'")
        VerifyNext(scanner, TokenType.CharacterAnsi, "'f'")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.CharacterAnsi, "'a'")
    End Sub

    <Fact>
    Public Sub BasicScan25()
        Dim scanner As Scanner = CreateScanner("'\n' '\10'")
        VerifyNext(scanner, TokenType.CharacterAnsi, "'\n'")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.CharacterAnsi, "'\10'")
    End Sub

    <Fact>
    Public Sub BasicScan26()
        Dim scanner As Scanner = CreateScanner("'\foo")
        VerifyNext(scanner, TokenType.SingleQuote)
        VerifyNext(scanner, TokenType.BackSlash)
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    <Fact>
    Public Sub BasicScan27()
        Dim scanner As Scanner = CreateScanner("foo'")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.SingleQuote)
    End Sub

    <Fact>
    Public Sub BasicScan28()
        Dim scanner As Scanner = CreateScanner("= ==")
        VerifyNext(scanner, TokenType.OpAssign, "=")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.OpEquals, "==")
    End Sub

    <Fact>
    Public Sub BasicScan29()
        Dim scanner As Scanner = CreateScanner("! !=")
        VerifyNext(scanner, TokenType.Bang, "!")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.OpNotEquals, "!=")
    End Sub

    <Fact>
    Public Sub BasicScan30()
        Dim scanner As Scanner = CreateScanner("_cdecl __cdecl __stdcall __pascal __winapi")
        VerifyNext(scanner, TokenType.CDeclarationCallKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.CDeclarationCallKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.StandardCallKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.PascalCallKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.WinApiCallKeyword)
    End Sub

    <Fact>
    Public Sub BasciScan31()
        Dim scanner As Scanner = CreateScanner("L'a' L1")
        VerifyNext(scanner, TokenType.CharacterUnicode, "L'a'")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Word, "L1")
    End Sub

    <Fact>
    Public Sub BasicScan32()
        Dim scanner As Scanner = CreateScanner("L'    5")
        VerifyNext(scanner, TokenType.Word, "L")
        VerifyNext(scanner, TokenType.SingleQuote)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Number, "5")
    End Sub

    <Fact>
    Public Sub BasicScan33()
        Dim scanner As Scanner = CreateScanner("public private protected class")
        VerifyNext(scanner, TokenType.PublicKeyword, "public")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.PrivateKeyword, "private")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.ProtectedKeyword, "protected")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.ClassKeyword, "class")
    End Sub

    <Fact>
    Public Sub BasicScan34()
        Dim scanner As Scanner = CreateScanner("signed unsigned")
        VerifyNext(scanner, TokenType.SignedKeyword, "signed")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.UnsignedKeyword, "unsigned")
    End Sub

    <Fact>
    Public Sub BasicScan35()
        Dim scanner As Scanner = CreateScanner("45i64 52ui64 -45i64")
        VerifyNext(scanner, TokenType.Number, "45i64")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Number, "52ui64")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.OpMinus)
        VerifyNext(scanner, TokenType.Number, "45i64")
    End Sub

    <Fact>
    Public Sub MultilineBasicScan1()
        Dim scanner As Scanner = CreateScanner("foo" & vbCrLf & "bar", _lineOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.NewLine)
        VerifyNext(scanner, TokenType.Word, "bar")
    End Sub

    <Fact>
    Public Sub MultilineBasicScan2()
        Dim scanner As Scanner = CreateScanner("foo," & vbCrLf & "bar[]", _lineOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.Comma)
        VerifyNext(scanner, TokenType.NewLine)
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.BracketOpen)
        VerifyNext(scanner, TokenType.BracketClose)
    End Sub

    <Fact>
    Public Sub MultilineBasicScan3()
        Dim scanner As Scanner = CreateScanner("bar,   " & vbCrLf & "foo", _lineOpts)
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.Comma)
        VerifyNext(scanner, TokenType.NewLine)
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    <Fact>
    Public Sub PeekToken1()
        Dim scanner As Scanner = CreateScanner("bar")
        VerifyPeek(scanner, TokenType.Word, "bar")
        VerifyPeek(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.Word, "bar")
    End Sub

    <Fact>
    Public Sub PeekToken2()
        Dim scanner As Scanner = CreateScanner("bar[]")
        VerifyPeek(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyPeek(scanner, TokenType.BracketOpen)
        VerifyNext(scanner, TokenType.BracketOpen)
        VerifyNext(scanner, TokenType.BracketClose)
    End Sub

    <Fact>
    Public Sub MarkAndRollback1()
        Dim scanner As Scanner = CreateScanner("bar()foo")
        VerifyNext(scanner, TokenType.Word, "bar")
        Dim mark As ScannerMark = scanner.Mark()
        VerifyNext(scanner, TokenType.ParenOpen)
        scanner.Rollback(mark)
        VerifyNext(scanner, TokenType.ParenOpen)
        VerifyNext(scanner, TokenType.ParenClose)
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    <Fact>
    Public Sub MarkAndRollback2()
        Dim scanner As Scanner = CreateScanner("bar[")
        Dim mark As ScannerMark = scanner.Mark()
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.BracketOpen)
        scanner.Rollback(mark)
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.BracketOpen)
    End Sub

    <Fact>
    Public Sub QuotedString1()
        Dim scanner As Scanner = CreateScanner("bar ""uu""", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """uu""")
    End Sub

    <Fact>
    Public Sub QuotedString2()
        Dim scanner As Scanner = CreateScanner("""a""""b""", _defaultOpts)
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """a""")
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """b""")
    End Sub

    <Fact>
    Public Sub QuotedString3()
        Dim scanner As Scanner = CreateScanner("""b""hello", _defaultOpts)
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """b""")
        VerifyNext(scanner, TokenType.Word, "hello")
    End Sub

    ''' <summary>
    ''' Test a string with a C++ escape sequence
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub QuotedString4()
        Dim scanner As Scanner = CreateScanner("""b\n""foo", _defaultOpts)
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """b\n""")
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    ''' <summary>
    ''' String with an escaped quote
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub QuotedString5()
        Dim scanner As Scanner = CreateScanner("""aaaa\""""bar", _defaultOpts)
        VerifyNext(scanner, TokenType.QuotedStringAnsi, """aaaa\""""")
        VerifyNext(scanner, TokenType.Word, "bar")
    End Sub

    <Fact>
    Public Sub QuotedString6()
        Dim scanner As Scanner = CreateScanner("L""foo"" L""")
        VerifyNext(scanner, TokenType.QuotedStringUnicode, "L""foo""")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Word, "L")
        VerifyNext(scanner, TokenType.DoubleQuote)
    End Sub

    <Fact>
    Public Sub LineComment1()
        Dim scanner As Scanner = CreateScanner("hello // bar", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "hello")
        VerifyNext(scanner, TokenType.LineComment, "// bar")
    End Sub

    ''' <summary>
    ''' Read a line comment that nests line comment characters
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub LineComment2()
        Dim scanner As Scanner = CreateScanner("foo // // bar", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.LineComment, "// // bar")
    End Sub

    ''' <summary>
    ''' Line comment that embeds block comments which shouldn't count
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub LineComment3()
        Dim scanner As Scanner = CreateScanner("foo // /* bar */", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.LineComment, "// /* bar */")
    End Sub

    ''' <summary>
    ''' Line comment at the start of a line
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub LineComment4()
        Dim scanner As Scanner = CreateScanner("// hello", _commentOpts)
        VerifyNext(scanner, TokenType.LineComment, "// hello")
    End Sub

    ''' <summary>
    ''' Empty line comment
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub LineComment5()
        Dim scanner As Scanner = CreateScanner("foo //", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.LineComment, "//")
    End Sub

    <Fact>
    Public Sub LineComment6()
        Dim scanner As Scanner = CreateScanner("foo //bar" & vbCrLf, New ScannerOptions())
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.WhiteSpace, " ")
        VerifyNext(scanner, TokenType.LineComment, "//bar")
        VerifyNext(scanner, TokenType.NewLine, vbCrLf)
    End Sub

    <Fact>
    Public Sub BlockComment1()
        Dim scanner As Scanner = CreateScanner("foo /* bar */", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.BlockComment, "/* bar */")
    End Sub

    <Fact>
    Public Sub BlockComment2()
        Dim scanner As Scanner = CreateScanner("foo /* bar */ a", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.BlockComment, "/* bar */")
        VerifyNext(scanner, TokenType.Word, "a")
    End Sub

    ''' <summary>
    ''' Several block comments
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub BlockComment3()
        Dim scanner As Scanner = CreateScanner("foo /* one */ /* two */", _commentOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.BlockComment, "/* one */")
        VerifyNext(scanner, TokenType.BlockComment, "/* two */")
    End Sub

    ''' <summary>
    ''' Make sure that the block comment will process when unclosed
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub BlockComment4()
        Dim scanner As Scanner = CreateScanner("/* foo", _commentOpts)
        VerifyNext(scanner, TokenType.BlockComment, "/* foo")
    End Sub

    <Fact>
    Public Sub NextOfType()
        Dim scanner As Scanner = CreateScanner("foo/* bar*/bar", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.Word, "bar")
    End Sub

    <Fact>
    Public Sub NextOfType2()
        Dim scanner As Scanner = CreateScanner("/*bar*/foo", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
    End Sub

    <Fact>
    Public Sub NextOfType3()
        Dim scanner As Scanner = CreateScanner("/*bar*/foo//hello", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.EndOfStream, String.Empty)
    End Sub

    <Fact>
    Public Sub PeekOfType()
        Dim scanner As Scanner = CreateScanner("bar/*foo*/", _defaultOpts)
        VerifyPeek(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyNext(scanner, TokenType.EndOfStream, String.Empty)
    End Sub

    <Fact>
    Public Sub PeekOfType2()
        Dim scanner As Scanner = CreateScanner("bar/*foo*/green", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "bar")
        VerifyPeek(scanner, TokenType.Word, "green")
        VerifyNext(scanner, TokenType.Word, "green")
    End Sub

    <Fact>
    Public Sub GetNextRealToken()
        Dim scanner As Scanner = CreateScanner("bar", _defaultOpts)
        scanner.GetNextToken(TokenType.Word)
    End Sub

    <Fact>
    Public Sub GetNextRealToken2()
        Dim scanner As Scanner = CreateScanner("bar", _defaultOpts)
        Assert.Throws(Of InvalidOperationException)(
            Sub()
                scanner.GetNextToken(TokenType.Asterisk)
            End Sub)
    End Sub

    <Fact>
    Public Sub NumberTest1()
        Dim scanner As Scanner = CreateScanner("foo2")
        VerifyNext(scanner, TokenType.Word, "foo2")
    End Sub

    <Fact>
    Public Sub NumberTest2()
        Dim scanner As Scanner = CreateScanner("foo 22", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.Number, "22")
    End Sub

    <Fact>
    Public Sub NumberTest3()
        Dim scanner As Scanner = CreateScanner("22foo 3.3", _defaultOpts)
        VerifyNext(scanner, TokenType.Word, "22foo")
        VerifyNext(scanner, TokenType.Number, "3.3")
    End Sub

    <Fact>
    Public Sub NumberTest4()
        Dim scanner As Scanner = CreateScanner("22L", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "22L")
    End Sub

    <Fact>
    Public Sub NumberTest5()
        Dim scanner As Scanner = CreateScanner("12u", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12u")
    End Sub

    <Fact>
    Public Sub NumberTest6()
        Dim scanner As Scanner = CreateScanner("12U", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12U")
    End Sub

    <Fact>
    Public Sub NumberTest7()
        Dim scanner As Scanner = CreateScanner("12UL", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12UL")
    End Sub

    ''' <summary>
    ''' Test the U option out
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub NumberTest8()
        Dim scanner As Scanner = CreateScanner("12U 0x5U 6u", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12U")
        VerifyNext(scanner, TokenType.HexNumber, "0x5U")
        VerifyNext(scanner, TokenType.Number, "6u")
    End Sub

    ''' <summary>
    ''' Play with the L option
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub NumberTest9()
        Dim scanner As Scanner = CreateScanner("12L 0x5L 6l", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12L")
        VerifyNext(scanner, TokenType.HexNumber, "0x5L")
        VerifyNext(scanner, TokenType.Number, "6l")
    End Sub

    ''' <summary>
    ''' Play with the F option
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub NumberTest10()
        Dim scanner As Scanner = CreateScanner("12F 0x5F 6f", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "12F")
        VerifyNext(scanner, TokenType.HexNumber, "0x5F")
        VerifyNext(scanner, TokenType.Number, "6f")
    End Sub

    ''' <summary>
    ''' Play with the exponnentials
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub NumberTest11()
        Dim scanner As Scanner = CreateScanner("1e2 0x5e5 7e2", _defaultOpts)
        VerifyNext(scanner, TokenType.Number, "1e2")
        VerifyNext(scanner, TokenType.HexNumber, "0x5e5")
        VerifyNext(scanner, TokenType.Number, "7e2")
    End Sub

    <Fact>
    Public Sub NumberTest12()
        Dim scanner As Scanner = CreateScanner("0x15")
        VerifyNext(scanner, TokenType.HexNumber, "0x15")
    End Sub

    <Fact>
    Public Sub NumberTest13()
        Dim scanner As Scanner = CreateScanner("1.0 1.a")
        VerifyNext(scanner, TokenType.Number, "1.0")
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.Number, "1")
        VerifyNext(scanner, TokenType.Period)
        VerifyNext(scanner, TokenType.Word, "a")
    End Sub

    <Fact>
    Public Sub NumberTestt14()
        Dim scanner As Scanner = CreateScanner("-0.1F")
        VerifyNext(scanner, TokenType.OpMinus)
        VerifyNext(scanner, TokenType.Number, "0.1F")
    End Sub

    <Fact>
    Public Sub BooleanTest1()
        Dim scanner As Scanner = CreateScanner("true false")
        VerifyNext(scanner, TokenType.TrueKeyword)
        VerifyNext(scanner, TokenType.WhiteSpace)
        VerifyNext(scanner, TokenType.FalseKeyword)
    End Sub


    ''' <summary>
    ''' Make sure it returns Whitespace
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub AllTest1()
        Dim scanner As Scanner = CreateScanner("foo 22")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.WhiteSpace, " ")
        VerifyNext(scanner, TokenType.Number, "22")
    End Sub

    ''' <summary>
    ''' Make sure it returns all of teh whitespace
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub AllTest2()
        Dim scanner As Scanner = CreateScanner("foo  22")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.WhiteSpace, "  ")
        VerifyNext(scanner, TokenType.Number, "22")
    End Sub

    ''' <summary>
    ''' More all whitespace tests
    ''' </summary>
    ''' <remarks></remarks>
    <Fact>
    Public Sub AllTest3()
        Dim scanner As Scanner = CreateScanner("foo  2. 2")
        VerifyNext(scanner, TokenType.Word, "foo")
        VerifyNext(scanner, TokenType.WhiteSpace, "  ")
        VerifyNext(scanner, TokenType.Number, "2.")
        VerifyNext(scanner, TokenType.WhiteSpace, " ")
        VerifyNext(scanner, TokenType.Number, "2")
    End Sub

    <Fact>
    Public Sub RudeEnd1()
        Dim scanner As Scanner = CreateScanner("foo", _rudeEndOpts)
        scanner.GetNextToken()
        Assert.Throws(Of EndOfStreamException)(Sub() scanner.GetNextToken())
    End Sub

    <Fact>
    Public Sub RudeEnd2()
        Dim scanner As Scanner = CreateScanner("foo bar", _rudeEndOpts)
        scanner.GetNextToken()
        scanner.GetNextToken()
        scanner.GetNextToken()
        Assert.Throws(Of EndOfStreamException)(Sub() scanner.GetNextToken())
    End Sub

End Class

