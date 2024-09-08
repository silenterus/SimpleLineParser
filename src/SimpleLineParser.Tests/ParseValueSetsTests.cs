using SimpleLineParser.Parser;
namespace SimpleLineParser.Tests;

using Data;
public class ParseValueSetsTests
{


    [Fact]
    public void ParseTables_ComplexInput_ParsesCorrectly()
    {


        // Arrange
        string rawText = """
                         Tester:enum.AddressTypesLocal,Global,Country,22.Regional(Only for, inner. country usage)
                         test.Tet:Hash(word),!Name(name),@Password(word,)
                         LastName(muh):Name(firstname(),LastName()lastname),Username(username.Name.LastName),Mail(email.Name.LastName),ExampleMail(example_email.Name.LastName),UserNameUnicode(user_name_unicode.Name.LastName)
                         22.Regional(Only for inner country usage):enum.Local,Global,Country,22.Regional(Only for inner country usage)
                         LastName:Hash(word),Password(word)
                         int.FullEmail(('Tester, the great')):Provider, int.FullEmail(('Tester, the great'))
                         Name(firstname):Name(firstname),LastName(lastname),Username(username.Name.LastName),Mail(email.Name.LastName),Email
                         Name:Name
                         double.Lat:double.Lat, double.Lon, Hash
                         !Prefix:Street, !Prefix, Gps, AddressTypes, Number
                         link.!Account(as):link.!Account(as),Address,2.!Login
                         22.Regional((((Only for inner country usage)))):string.Local,int.Global,Country,22.Regional((((Only for inner country usage))))
                         Name:Name
                         Account:Name(firstname),LastName(lastname),Username(username.Name.LastName),Mail(email.Name.LastName)
                         ip.0.24.241.2(34433)
                         Db.PersonalData(OllamaBridge.Core.Database)
                         db.PersonalData(OllamaBridge.Core.Database)
                         api.Db.PersonalData(OllamaBridge.Core.Database)
                         """;

        // Act
        var resultsRaw = ParseTables(rawText);

        // Assert
        Assert.Equal(18, resultsRaw[0].Segments.Count);

        // ValueSet 1
        var set1 = resultsRaw[0].Segments[0];
        Assert.Equal("Tester", set1.Left[0].Name);
        Assert.Equal("", set1.Left[0].Prefix);
        Assert.Equal("", set1.Left[0].Value);
        Assert.Equal(0, set1.Left[0].Options[0]);
        Assert.Equal(4, set1.Right.Count);
        Assert.Equal("AddressTypesLocal", set1.Right[0].Name);
        Assert.Equal("enum", set1.Right[0].Prefix);
        Assert.Equal("", set1.Right[0].Value);
        Assert.Equal(0, set1.Right[0].Options[0]);
        Assert.Equal("Global", set1.Right[1].Name);
        Assert.Equal("", set1.Right[1].Prefix);
        Assert.Equal("", set1.Right[1].Value);
        Assert.Equal(0, set1.Right[1].Options[0]);
        Assert.Equal("Country", set1.Right[2].Name);
        Assert.Equal("", set1.Right[2].Prefix);
        Assert.Equal("", set1.Right[2].Value);
        Assert.Equal(0, set1.Right[2].Options[0]);
        Assert.Equal("Regional", set1.Right[3].Name);
        Assert.Equal("22", set1.Right[3].Prefix);
        Assert.Equal("Only for, inner. country usage", set1.Right[3].Value);
        Assert.Equal(0, set1.Right[3].Options[0]);

        // ValueSet 2
        var set2 = resultsRaw[0].Segments[1];
        Assert.Equal("Tet", set2.Left[0].Name);
        Assert.Equal("test", set2.Left[0].Prefix);
        Assert.Equal("", set2.Left[0].Value);
        Assert.Equal(0, set2.Left[0].Options[0]);
        Assert.Equal(3, set2.Right.Count);
        Assert.Equal("Hash", set2.Right[0].Name);
        Assert.Equal("", set2.Right[0].Prefix);
        Assert.Equal("word", set2.Right[0].Value);
        Assert.Equal(0, set2.Right[0].Options[0]);
        Assert.Equal("Name", set2.Right[1].Name);
        Assert.Equal("", set2.Right[1].Prefix);
        Assert.Equal("name", set2.Right[1].Value);
        Assert.Equal(1, set2.Right[1].Options[0]);
        Assert.Equal("Password", set2.Right[2].Name);
        Assert.Equal("", set2.Right[2].Prefix);
        Assert.Equal("word,", set2.Right[2].Value);
        Assert.Equal(2, set2.Right[2].Options[0]);

        // ValueSet 3
        var set3 = resultsRaw[0].Segments[2];
        Assert.Equal("LastName", set3.Left[0].Name);
        Assert.Equal("", set3.Left[0].Prefix);
        Assert.Equal("muh", set3.Left[0].Value);
        Assert.Equal(0, set3.Left[0].Options[0]);
        Assert.Equal(5, set3.Right.Count);
        Assert.Equal("Name", set3.Right[0].Name);
        Assert.Equal("", set3.Right[0].Prefix);
        Assert.Equal("firstname(),LastName()lastname", set3.Right[0].Value);
        Assert.Equal(0, set3.Right[0].Options[0]);
        Assert.Equal("Username", set3.Right[1].Name);
        Assert.Equal("", set3.Right[1].Prefix);
        Assert.Equal("username.Name.LastName", set3.Right[1].Value);
        Assert.Equal(0, set3.Right[1].Options[0]);
        Assert.Equal("Mail", set3.Right[2].Name);
        Assert.Equal("", set3.Right[2].Prefix);
        Assert.Equal("email.Name.LastName", set3.Right[2].Value);
        Assert.Equal(0, set3.Right[2].Options[0]);
        Assert.Equal("ExampleMail", set3.Right[3].Name);
        Assert.Equal("", set3.Right[3].Prefix);
        Assert.Equal("example_email.Name.LastName", set3.Right[3].Value);
        Assert.Equal(0, set3.Right[3].Options[0]);
        Assert.Equal("UserNameUnicode", set3.Right[4].Name);
        Assert.Equal("", set3.Right[4].Prefix);
        Assert.Equal("user_name_unicode.Name.LastName", set3.Right[4].Value);
        Assert.Equal(0, set3.Right[4].Options[0]);

        // ValueSet 4
        var set4 = resultsRaw[0].Segments[3];
        Assert.Equal("Regional", set4.Left[0].Name);
        Assert.Equal("22", set4.Left[0].Prefix);
        Assert.Equal("Only for inner country usage", set4.Left[0].Value);
        Assert.Equal(0, set4.Left[0].Options[0]);
        Assert.Equal(4, set4.Right.Count);
        Assert.Equal("Local", set4.Right[0].Name);
        Assert.Equal("enum", set4.Right[0].Prefix);
        Assert.Equal("", set4.Right[0].Value);
        Assert.Equal(0, set4.Right[0].Options[0]);
        Assert.Equal("Global", set4.Right[1].Name);
        Assert.Equal("", set4.Right[1].Prefix);
        Assert.Equal("", set4.Right[1].Value);
        Assert.Equal(0, set4.Right[1].Options[0]);
        Assert.Equal("Country", set4.Right[2].Name);
        Assert.Equal("", set4.Right[2].Prefix);
        Assert.Equal("", set4.Right[2].Value);
        Assert.Equal(0, set4.Right[2].Options[0]);
        Assert.Equal("Regional", set4.Right[3].Name);
        Assert.Equal("22", set4.Right[3].Prefix);
        Assert.Equal("Only for inner country usage", set4.Right[3].Value);
        Assert.Equal(0, set4.Right[3].Options[0]);

        // ValueSet 5
        var set5 = resultsRaw[0].Segments[4];
        Assert.Equal("LastName", set5.Left[0].Name);
        Assert.Equal("", set5.Left[0].Prefix);
        Assert.Equal("", set5.Left[0].Value);
        Assert.Equal(0, set5.Left[0].Options[0]);
        Assert.Equal(2, set5.Right.Count);
        Assert.Equal("Hash", set5.Right[0].Name);
        Assert.Equal("", set5.Right[0].Prefix);
        Assert.Equal("word", set5.Right[0].Value);
        Assert.Equal(0, set5.Right[0].Options[0]);
        Assert.Equal("Password", set5.Right[1].Name);
        Assert.Equal("", set5.Right[1].Prefix);
        Assert.Equal("word", set5.Right[1].Value);
        Assert.Equal(0, set5.Right[1].Options[0]);

        // ValueSet 6
        var set6 = resultsRaw[0].Segments[5];
        Assert.Equal("FullEmail", set6.Left[0].Name);
        Assert.Equal("int", set6.Left[0].Prefix);
        Assert.Equal("('Tester, the great')", set6.Left[0].Value);
        Assert.Equal(0, set6.Left[0].Options[0]);
        Assert.Equal(2, set6.Right.Count);
        Assert.Equal("Provider", set6.Right[0].Name);
        Assert.Equal("", set6.Right[0].Prefix);
        Assert.Equal("", set6.Right[0].Value);
        Assert.Equal(0, set6.Right[0].Options[0]);
        Assert.Equal("FullEmail", set6.Right[1].Name);
        Assert.Equal("int", set6.Right[1].Prefix);
        Assert.Equal("('Tester, the great')", set6.Right[1].Value);
        Assert.Equal(0, set6.Right[1].Options[0]);



            // ValueSet 7
        var set7 = resultsRaw[0].Segments[6];
        Assert.Equal("Name", set7.Left[0].Name);
        Assert.Equal("", set7.Left[0].Prefix);
        Assert.Equal("firstname", set7.Left[0].Value);
        Assert.Equal(0, set7.Left[0].Options[0]);
        Assert.Equal(5, set7.Right.Count);
        Assert.Equal("Name", set7.Right[0].Name);
        Assert.Equal("", set7.Right[0].Prefix);
        Assert.Equal("firstname", set7.Right[0].Value);
        Assert.Equal(0, set7.Right[0].Options[0]);
        Assert.Equal("LastName", set7.Right[1].Name);
        Assert.Equal("", set7.Right[1].Prefix);
        Assert.Equal("lastname", set7.Right[1].Value);
        Assert.Equal(0, set7.Right[1].Options[0]);
        Assert.Equal("Username", set7.Right[2].Name);
        Assert.Equal("", set7.Right[2].Prefix);
        Assert.Equal("username.Name.LastName", set7.Right[2].Value);
        Assert.Equal(0, set7.Right[2].Options[0]);
        Assert.Equal("Mail", set7.Right[3].Name);
        Assert.Equal("", set7.Right[3].Prefix);
        Assert.Equal("email.Name.LastName", set7.Right[3].Value);
        Assert.Equal(0, set7.Right[3].Options[0]);
        Assert.Equal("Email", set7.Right[4].Name);
        Assert.Equal("", set7.Right[4].Prefix);
        Assert.Equal("", set7.Right[4].Value);
        Assert.Equal(0, set7.Right[4].Options[0]);

        // ValueSet 8
        var set8 = resultsRaw[0].Segments[7];
        Assert.Equal("Name", set8.Left[0].Name);
        Assert.Equal("", set8.Left[0].Prefix);
        Assert.Equal("", set8.Left[0].Value);
        Assert.Equal(0, set8.Left[0].Options[0]);
        Assert.Equal(1, set8.Right.Count);
        Assert.Equal("Name", set8.Right[0].Name);
        Assert.Equal("", set8.Right[0].Prefix);
        Assert.Equal("", set8.Right[0].Value);
        Assert.Equal(0, set8.Right[0].Options[0]);

        // ValueSet 9
        var set9 = resultsRaw[0].Segments[8];
        Assert.Equal("Lat", set9.Left[0].Name);
        Assert.Equal("double", set9.Left[0].Prefix);
        Assert.Equal("", set9.Left[0].Value);
        Assert.Equal(0, set9.Left[0].Options[0]);
        Assert.Equal(3, set9.Right.Count);
        Assert.Equal("Lat", set9.Right[0].Name);
        Assert.Equal("double", set9.Right[0].Prefix);
        Assert.Equal("", set9.Right[0].Value);
        Assert.Equal(0, set9.Right[0].Options[0]);
        Assert.Equal("Lon", set9.Right[1].Name);
        Assert.Equal("double", set9.Right[1].Prefix);
        Assert.Equal("", set9.Right[1].Value);
        Assert.Equal(0, set9.Right[1].Options[0]);
        Assert.Equal("Hash", set9.Right[2].Name);
        Assert.Equal("", set9.Right[2].Prefix);
        Assert.Equal("", set9.Right[2].Value);
        Assert.Equal(0, set9.Right[2].Options[0]);

        // ValueSet 10
        var set10 = resultsRaw[0].Segments[9];
        Assert.Equal("Prefix", set10.Left[0].Name);
        Assert.Equal("", set10.Left[0].Prefix);
        Assert.Equal("", set10.Left[0].Value);
        Assert.Equal(1, set10.Left[0].Options[0]);

        Assert.Equal(5, set10.Right.Count);
        Assert.Equal("Street", set10.Right[0].Name);
        Assert.Equal("", set10.Right[0].Prefix);
        Assert.Equal("", set10.Right[0].Value);
        Assert.Equal(0, set10.Right[0].Options[0]);
        Assert.Equal("Prefix", set10.Right[1].Name);
        Assert.Equal("", set10.Right[1].Prefix);
        Assert.Equal("", set10.Right[1].Value);
        Assert.Equal(1, set10.Right[1].Options[0]);
        Assert.Equal("Gps", set10.Right[2].Name);
        Assert.Equal("", set10.Right[2].Prefix);
        Assert.Equal("", set10.Right[2].Value);
        Assert.Equal(0, set10.Right[2].Options[0]);
        Assert.Equal("AddressTypes", set10.Right[3].Name);
        Assert.Equal("", set10.Right[3].Prefix);
        Assert.Equal("", set10.Right[3].Value);
        Assert.Equal(0, set10.Right[3].Options[0]);
        Assert.Equal("Number", set10.Right[4].Name);
        Assert.Equal("", set10.Right[4].Prefix);
        Assert.Equal("", set10.Right[4].Value);
        Assert.Equal(0, set10.Right[4].Options[0]);

        // ValueSet 11
        var set11 = resultsRaw[0].Segments[10];
        Assert.Equal("Account", set11.Left[0].Name);
        Assert.Equal("link", set11.Left[0].Prefix);
        Assert.Equal("as", set11.Left[0].Value);
        Assert.Equal(1, set11.Left[0].Options[0]);
        Assert.Equal(3, set11.Right.Count);
        Assert.Equal("Account", set11.Right[0].Name);
        Assert.Equal("link", set11.Right[0].Prefix);
        Assert.Equal("as", set11.Right[0].Value);
        Assert.Equal(1, set11.Right[0].Options[0]);
        Assert.Equal("Address", set11.Right[1].Name);
        Assert.Equal("", set11.Right[1].Prefix);
        Assert.Equal("", set11.Right[1].Value);
        Assert.Equal(0, set11.Right[1].Options[0]);
        Assert.Equal("Login", set11.Right[2].Name);
        Assert.Equal("2", set11.Right[2].Prefix);
        Assert.Equal("", set11.Right[2].Value);
        Assert.Equal(1, set11.Right[2].Options[0]);

        // ValueSet 12
        var set12 = resultsRaw[0].Segments[11];
        Assert.Equal("Regional", set12.Left[0].Name);
        Assert.Equal("22", set12.Left[0].Prefix);
        Assert.Equal("(((Only for inner country usage)))", set12.Left[0].Value);
        Assert.Equal(0, set12.Left[0].Options[0]);
        Assert.Equal(4, set12.Right.Count);
        Assert.Equal("Local", set12.Right[0].Name);
        Assert.Equal("string", set12.Right[0].Prefix);
        Assert.Equal("", set12.Right[0].Value);
        Assert.Equal(0, set12.Right[0].Options[0]);
        Assert.Equal("Global", set12.Right[1].Name);
        Assert.Equal("int", set12.Right[1].Prefix);
        Assert.Equal("", set12.Right[1].Value);
        Assert.Equal(0, set12.Right[1].Options[0]);
        Assert.Equal("Country", set12.Right[2].Name);
        Assert.Equal("", set12.Right[2].Prefix);
        Assert.Equal("", set12.Right[2].Value);
        Assert.Equal(0, set12.Right[2].Options[0]);
        Assert.Equal("Regional", set12.Right[3].Name);
        Assert.Equal("22", set12.Right[3].Prefix);
        Assert.Equal("(((Only for inner country usage)))", set12.Right[3].Value);
        Assert.Equal(0, set12.Right[3].Options[0]);

        // ValueSet 13
        var set14 = resultsRaw[0].Segments[12];
        Assert.Equal("Name", set14.Left[0].Name);
        Assert.Equal("", set14.Left[0].Prefix);
        Assert.Equal("", set14.Left[0].Value);
        Assert.Equal(0, set14.Left[0].Options[0]);
        Assert.Single(set14.Right);
        Assert.Equal("Name", set14.Right[0].Name);
        Assert.Equal("", set14.Right[0].Prefix);
        Assert.Equal("", set14.Right[0].Value);
        Assert.Equal(0, set14.Right[0].Options[0]);

        // ValueSet 14
        var set13 = resultsRaw[0].Segments[13];
        Assert.Equal("Account", set13.Left[0].Name);
        Assert.Equal("", set13.Left[0].Prefix);
        Assert.Equal("", set13.Left[0].Value);
        Assert.Equal(0, set13.Left[0].Options[0]);
        Assert.Equal(4, set13.Right.Count);
        Assert.Equal("Name", set13.Right[0].Name);
        Assert.Equal("", set13.Right[0].Prefix);
        Assert.Equal("firstname", set13.Right[0].Value);
        Assert.Equal(0, set13.Right[0].Options[0]);
        Assert.Equal("LastName", set13.Right[1].Name);
        Assert.Equal("", set13.Right[1].Prefix);
        Assert.Equal("lastname", set13.Right[1].Value);
        Assert.Equal(0, set13.Right[1].Options[0]);
        Assert.Equal("Username", set13.Right[2].Name);
        Assert.Equal("", set13.Right[2].Prefix);
        Assert.Equal("username.Name.LastName", set13.Right[2].Value);
        Assert.Equal(0, set13.Right[2].Options[0]);
        Assert.Equal("Mail", set13.Right[3].Name);
        Assert.Equal("", set13.Right[3].Prefix);
        Assert.Equal("email.Name.LastName", set13.Right[3].Value);
        Assert.Equal(0, set13.Right[3].Options[0]);



        /*
        // ValueSet 15
        var set15 = resultsRaw[0].Segments[14];
        Assert.Equal("0.24.241.2", set15.Left[0].Name);
        Assert.Equal("ip", set15.Left[0].Prefix);
        Assert.Equal("34433", set15.Left[0].Value);
        Assert.Equal(0, set15.Left[0].Options[0]);
        Assert.Empty(set15.Right);
        */



        // ValueSet 16

        var set16 = resultsRaw[0].Segments[15];
        Assert.Equal("Db.PersonalData", set16.Left[0].Name);
        Assert.Equal("", set16.Left[0].Prefix);
        Assert.Equal("OllamaBridge.Core.Database", set16.Left[0].Value);
        Assert.Equal(0, set16.Left[0].Options[0]);
        Assert.Empty(set16.Right);



        // ValueSet 17
        var set17 = resultsRaw[0].Segments[16];
        Assert.Equal("PersonalData", set17.Left[0].Name);
        Assert.Equal("db", set17.Left[0].Prefix);
        Assert.Equal("OllamaBridge.Core.Database", set17.Left[0].Value);
        Assert.Equal(0, set17.Left[0].Options[0]);
        Assert.Empty(set17.Right);




        // ValueSet 18
        var set18 = resultsRaw[0].Segments[17];
        Assert.Equal("Db.PersonalData", set18.Left[0].Name);
        Assert.Equal("api", set18.Left[0].Prefix);
        Assert.Equal("OllamaBridge.Core.Database", set18.Left[0].Value);
        Assert.Equal(0, set18.Left[0].Options[0]);
        Assert.Empty(set18.Right);



    }

    [Fact]
    public void ParseComplexEntries_ShouldHandleMultipleFormatsAndNesting()
    {
        string rawText = """
                         Tester:enum.AddressTypesLocal,Global,Country,22.Regional(Only for, inner. country usage),
                         test.Tet:Hash(word),!Name(name),@Password(word,),
                         LastName(muh):Name(firstname(),LastName()lastname),Username(username.Name.LastName),Mail(email.Name.LastName),ExampleMail(example_email.Name.LastName),UserNameUnicode(user_name_unicode.Name.LastName)
                         22.Regional(Only for inner country usage):enum.Local,Global,Country,22.Regional(Only for inner country usage),
                         LastName:Hash(word),Password(word),
                         int.FullEmail(('Tester, the great')):Provider, int.FullEmail(('Tester, the great'))
                         Name(firstname):Name(firstname),LastName(lastname),Username(username.Name.LastName),Mail(email.Name.LastName),Email,
                         Name:Name,
                         double.Lat:double.Lat, double.Lon, Hash,
                         !Prefix:Street, !Prefix, Gps, AddressTypes, Number,
                         link.!Account(as):link.!Account(as),Address,2.!Login,
                         22.Regional((((Only for inner country usage)))):string.Local,int.Global,Country,22.Regional((((Only for inner country usage))))
                         Name:Name,
                         Account:Name(firstname),LastName(lastname),Username(username.Name.LastName),Mail(email.Name.LastName)
                         """;

        var results = ParseTables(rawText);
        Assert.NotEmpty(results);
        Assert.Equal(14, results[0].Segments.Count); // Should equal the number of distinct top-level entries

        // Specific checks for a few selected entries to validate complex parsing logic
        Assert.Equal("Tester", results[0].Segments[0].Left[0].Name);
//        Assert.Equal("22", results[0].Segments[0].Right[3].Prefix);
        Assert.Equal("Regional", results[0].Segments[0].Right[3].Name);
        Assert.Equal("Only for, inner. country usage", results[0].Segments[0].Right[3].Value);

        Assert.Equal("test", results[0].Segments[1].Left[0].Prefix);
        Assert.Equal("Password", results[0].Segments[1].Right[2].Name);
        Assert.Equal("word,", results[0].Segments[1].Right[2].Value);
        Assert.Equal(2, results[0].Segments[1].Right[2].Options[0]); // Options[0] for '@'

        Assert.Equal("int", results[0].Segments[5].Left[0].Prefix);
        Assert.Equal("Provider", results[0].Segments[5].Right[0].Name);

        Assert.Equal("double", results[0].Segments[8].Left[0].Prefix);
        Assert.Equal("Lat", results[0].Segments[8].Left[0].Name);

        Assert.Equal("double", results[0].Segments[8].Right[0].Prefix);
        Assert.Equal("Lat", results[0].Segments[8].Right[0].Name);
        Assert.Equal("Lon", results[0].Segments[8].Right[1].Name);

        Assert.Equal("link", results[0].Segments[10].Right[0].Prefix);
        Assert.Equal("Account", results[0].Segments[10].Right[0].Name);
        Assert.Equal("as", results[0].Segments[10].Right[0].Value);
        Assert.Equal(1, results[0].Segments[10].Right[0].Options[0]); // Options[0] for '!'
    }

    [Fact]
    public void ParseComplexEntries_ShouldCorrectlyHandleSpecialCharacters()
    {
        string rawText = """
                         link.!Account(as):link.!Account(as),Address,2.!Login
                         22.Regional((((Only for inner country usage)))):string.Local,int.Global,Country,22.Regional((((Only for inner country usage))))
                         """;

        var results = ParseTables(rawText);
        Assert.NotEmpty(results);
        Assert.Equal(2, results[0].Segments.Count); // Should equal the number of entries

        // Checking handling of special characters and nested parenthesis
        Assert.Equal("Account", results[0].Segments[0].Right[0].Name);
        Assert.Equal("as", results[0].Segments[0].Right[0].Value);
        Assert.Equal("Login", results[0].Segments[0].Right[2].Name);
        Assert.Equal(1, results[0].Segments[0].Right[2].Options[0]); // Options[0] for '2.!'

        Assert.Equal("22", results[0].Segments[1].Left[0].Prefix);
        Assert.Equal("Regional", results[0].Segments[1].Left[0].Name);
        Assert.Equal("(((Only for inner country usage)))", results[0].Segments[1].Left[0].Value);
    }


    [Fact]
    public void ParseProgram_EmptyInput_ReturnsEmptyList()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = ParseProgram(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseProgram_ValidInput_ReturnsParsedValues()
    {
        // Arrange
        var input = "prefix.!Name(Value)";

        // Act
        var result = ParseProgram(input);

        // Assert
        Assert.Single(result);
        var set = result[0];
        Assert.Equal("prefix", set.Segments[0].Left[0].Prefix);
        Assert.Equal("Name", set.Segments[0].Left[0].Name);
        Assert.Equal("Value", set.Segments[0].Left[0].Value);
        Assert.Equal(1, set.Segments[0].Left[0].Options[0]); // ! maps to 1
    }

    [Fact]
    public void ParseDatabase_EmptyInput_ReturnsEmptyList()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = ParseDatabase(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseDatabase_ValidInput_ReturnsParsedValues()
    {
        // Arrange
        var input = "prefix.@Name(Value)";

        // Act
        var result = ParseDatabase(input);

        // Assert
        Assert.Single(result);
        var set = result[0];
        Assert.Equal("prefix", set.Segments[0].Left[0].Prefix);
        Assert.Equal("Name", set.Segments[0].Left[0].Name);
        Assert.Equal("Value", set.Segments[0].Left[0].Value);
        Assert.Equal(2, set.Segments[0].Left[0].Options[0]); // @ maps to 2
    }

    [Fact]
    public void ParseTables_EmptyInput_ReturnsEmptyList()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = ParseTables(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseTables_ValidInput_ReturnsValidSet()
    {
        var inputEmpty= @"

LeftName

";

        var resultWithEmpty = ParseTables(inputEmpty)[0];
        var set1 = resultWithEmpty.Segments[0];
        Assert.Equal("", set1.Left[0].Prefix);
        Assert.Equal("LeftName", set1.Left[0].Name);
        Assert.Equal("", set1.Left[0].Value);
        Assert.Equal(0, set1.Left[0].Options[0]);


        // Arrange
        var inputEmptyLeft = @"

LeftName:RightName

";

        // Act
        var resultWithEmptyLeft = ParseTables(inputEmptyLeft)[0];
        var set2 = resultWithEmptyLeft.Segments[0];
        Assert.Equal("", set2.Left[0].Prefix);
        Assert.Equal("LeftName", set2.Left[0].Name);
        Assert.Equal("", set2.Left[0].Value);
        Assert.Equal(0, set2.Left[0].Options[0]);

        Assert.Equal("", set2.Right[0].Prefix);
        Assert.Equal("RightName", set2.Right[0].Name);
        Assert.Equal("", set2.Right[0].Value);
        Assert.Equal(0, set2.Right[0].Options[0]);

        // Arrange
        var input = @"

LeftName:RightName

";

        // Act
        var result = ParseTables(input)[0];

        // Assert
        //Assert.Single(result);

        var set3 = result.Segments[0];
        Assert.Equal("", set3.Left[0].Prefix);
        Assert.Equal("LeftName", set3.Left[0].Name);
        Assert.Equal("", set3.Left[0].Value);
        Assert.Equal(0, set3.Left[0].Options[0]);

        Assert.Equal("", set3.Right[0].Prefix);
        Assert.Equal("RightName", set3.Right[0].Name);
        Assert.Equal("", set3.Right[0].Value);
        Assert.Equal(0, set3.Right[0].Options[0]);

    }


    [Fact]
    public void ParseTables_ValidInput_ReturnsParsedValues()
    {
        // Arrange
        var input = @"
prefix.*Name(Value)
prefix.+Name(Value)
prefix.#Name(Value)
";

        // Act
        var result = ParseTables(input)[0];

        // Assert
        //Assert.Single(result);

        var set = result.Segments[0];
        Assert.Equal("prefix", set.Left[0].Prefix);
        Assert.Equal("Name", set.Left[0].Name);
        Assert.Equal("Value", set.Left[0].Value);
        Assert.Equal(3, set.Left[0].Options[0]);

        var set2 = result.Segments[1];
        Assert.Equal("prefix", set2.Left[0].Prefix);
        Assert.Equal("Name", set2.Left[0].Name);
        Assert.Equal("Value", set2.Left[0].Value);
        Assert.Equal(4, set2.Left[0].Options[0]);

        var set3 =  result.Segments[2];
        Assert.Equal("prefix", set3.Left[0].Prefix);
        Assert.Equal("Name", set3.Left[0].Name);
        Assert.Equal("Value", set3.Left[0].Value);
        Assert.Equal(5, set3.Left[0].Options[0]);
    }

    // Helper methods for accessing the static methods
    private List<SetParse> ParseProgram(string raw) => Scheme.Parse(raw);
    private List<SetParse> ParseDatabase(string raw) => Scheme.Parse(raw);
    private List<SetParse> ParseTables(string raw) => Scheme.Parse(raw);
}
