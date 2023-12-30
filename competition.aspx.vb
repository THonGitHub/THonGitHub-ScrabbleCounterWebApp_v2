Imports System.Numerics
Imports MySql.Data.MySqlClient
''' <summary>
''' Na stránke sa zobrazí meno hráča na ťahu, ktoré je totožné s menom na ktoré sa kliklo
''' na stránke default1.aspx pre spustenie počítadla.
''' To isté meno sa zobrazí ako meno hráča, ktorý začal hru. Toto meno sa počas celej hry nemení.
''' Meno hráča na ťahu sa strieda podľa toho, ktorý hráč je na ťahu.
''' V poli "Číslo ťahu" sa zobrazuje a postupne zvyšuje aktuálne číslo ťahu hráčov.
''' V časti "Priebežný stav hry" sa pre každého z hráčov zobrazuje hodnota akutálne zadaného
''' slova, zároveň súčet hodnôt slov od začiatku hry a taktiež najvyššia hodnota doteray zadaných slov.
''' Na konci hry treba zadať hodnoty za nepoužité písmená (ako záporné čísla) a tieto sa odpočítajú
''' z celkového súčtu bodov každého hráča.
''' Stlačením tlačítka "Koniec hry a vyhodnotenie výsledkov" sa zobrazí stránka results.aspx.
''' </summary>
''' <param name="sender"></param>
''' <param name="e"></param>

Public Class competition
    Inherits System.Web.UI.Page
    Public query As String
    Public player_ID_from_QueryString As Integer
    Public player_Name_from_QueryString As String
    Public selectedPlayersList As List(Of SelectedPlayer)
    Public por_cislo As Integer

    ''' <summary>
    ''' čo sa vykoná pri načítaní stránky competition.aspx
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>


    'original
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Label_pomocny_text1.Text = "por_cislo ="

        If Not IsPostBack Then
            If Request.QueryString("playerID") IsNot Nothing AndAlso Request.QueryString("playerName") IsNot Nothing Then
                player_ID_from_QueryString = Convert.ToInt32(Request.QueryString("playerID"))
                player_Name_from_QueryString = Request.QueryString("playerName")

                Label_meno_hraca_na_tahu.Text = player_Name_from_QueryString
                Label_tuto_hru_zacal.Text = player_Name_from_QueryString

                selectedPlayersList = DirectCast(Session("SelectedPlayersList"), List(Of SelectedPlayer))
                por_cislo = 1
                If selectedPlayersList IsNot Nothing AndAlso selectedPlayersList.Count > 0 Then
                    For Each selectedPlayer As SelectedPlayer In selectedPlayersList

                        AddRowToTable(por_cislo, selectedPlayer.PlayerID, selectedPlayer.PlayerName, 0, 0, 0)
                        por_cislo += 1
                    Next
                Else
                    Label_1sprava.Text = "žiadne dáta"
                End If
            End If
        End If

        Label_pomocny_text2.Text = por_cislo

        'zaistiť, že pole na zadávanie bodov bude vždy aktívne
        If ScriptManager1.IsInAsyncPostBack Then
            ScriptManager.RegisterStartupScript(TextBox_zadat_body_za_slovo, TextBox_zadat_body_za_slovo.GetType(), "SetFocusScript", "$get('" & TextBox_zadat_body_za_slovo.ClientID & "').focus();", True)
        Else
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "SetFocusScript", "$get('" & TextBox_zadat_body_za_slovo.ClientID & "').focus();", True)
        End If

    End Sub


    ''' <summary>
    ''' čo sa má vykonať po zadaní čísla a stlačení ENTER - zavolať funkciu "Pridat_body_za_slovo"
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub TextBox_zadat_body_za_slovo_TextChanged(sender As Object, e As EventArgs)

        Dim input As String = TextBox_zadat_body_za_slovo.Text.Trim()

        'skontrolovať, či na vstupe bolo zadané číslo alebo písmeno
        Dim input_INT As Integer
        Dim input_Char As Char
        Dim isChar As Boolean = Char.TryParse(input, input_Char)
        Dim isInteger As Boolean = Integer.TryParse(input, input_INT)

        'ak bol zadaný INTEGER
        If isInteger Then
            If input_INT <> 0 Then

                Pridat_body_za_slovo(input)

            ElseIf input_INT = 0 Then

                Label_2sprava.Text = "Zadal si 0. Bude hráč v tomto kole stáť? Vyber a/n"
                Label_2sprava.Visible = True
            End If

            'ak bolo zadané písmeno
        ElseIf isChar Then

            If input.Length = 1 Then

                'ak sa stlačilo a - znamená to, že hráč stojí a priráta sa mu nula bodov
                If input.ToLower = "a" Then
                    Label_2sprava.Text = "Stlačil si [a], hráč bude stáť."
                    Label_2sprava.Visible = True
                    'hráčovi sa priráto 0 bodov
                    Pridat_body_za_slovo(0)
                    Label_2sprava.Visible = False

                    'ak sa stlačilo n, znamená to, že hráč nestojí a pokračuje v hre
                ElseIf input.ToLower = "n" Then
                    Label_2sprava.Text = "Stlačil si [n], hráč pokračuje v hre."

                End If
                Label_2sprava.Visible = False
            End If
        End If

        ' Label_cislo_tahu.Text = cislo_tahu.ToString()
        TextBox_zadat_body_za_slovo.Text = ""
    End Sub


    ''' <summary>
    ''' Počas prídávania bodov vykonávať aj ďalšie operácie:
    ''' - pripočítavať body
    ''' - porovnávať hodnoty a meniť pozadie čísel
    ''' - nulovať pomocné počítadlá
    ''' </summary>
    ''' <param name="Input"></param>


    '22.odpoved
    Protected Sub Pridat_body_za_slovo(Input As String)
        ' Retrieve the list of selected players from session
        selectedPlayersList = TryCast(Session("SelectedPlayersList"), List(Of SelectedPlayer))

        ' Check if the list is not null and contains players
        If selectedPlayersList IsNot Nothing AndAlso selectedPlayersList.Count > 0 Then
            Dim por_cislo As Integer = 1 ' Initialize por_cislo here or get it from the table row if it's displayed

            ' Update the current player's data based on the input
            If Integer.TryParse(Input, Nothing) Then
                selectedPlayersList(por_cislo - 1).BodyZaSlovo = Convert.ToInt32(Input)
                selectedPlayersList(por_cislo - 1).BodySpolu += Convert.ToInt32(Input)

                ' Update the table with the modified player data
                AddRowToTable(por_cislo, selectedPlayersList(por_cislo - 1).PlayerID, selectedPlayersList(por_cislo - 1).PlayerName, selectedPlayersList(por_cislo - 1).BodyZaSlovo, selectedPlayersList(por_cislo - 1).BodySpolu, selectedPlayersList(por_cislo - 1).TopBodyZaSlovo)
            End If
        End If

        TextBox_zadat_body_za_slovo.Text = ""
    End Sub


    ''' <summary>
    ''' pridať riadok s menom hráča do tabuľky
    ''' </summary>
    ''' <param name="player_ID"></param>
    ''' <param name="player_name"></param>
    ''' <param name="body_za_slovo"></param>
    ''' <param name="body_spolu"></param>
    ''' <param name="top_body_za_slovo"></param>
    Private Sub AddRowToTable(por_cislo As Integer, player_ID As Integer, player_name As String, body_za_slovo As Integer, body_spolu As Integer, top_body_za_slovo As Integer)
        Dim row As New TableRow()

        ' vytvoriť bunky pre ID a meno hráča
        Dim cell_PlayerID As New TableCell() With {
        .Text = player_ID.ToString(),
        .BorderStyle = BorderStyle.Solid,
        .BorderWidth = Unit.Pixel(1)
        }

        Dim cell_PlayerName As New TableCell() With {
        .Text = player_name,
        .BorderStyle = BorderStyle.Solid,
        .BorderWidth = Unit.Pixel(1)
        }

        ' dynamicky vytvoriť ASP labels
        Dim label_por_cislo As New Label() With {
        .ID = "Label_por_cislo_" & por_cislo,
        .Text = por_cislo.ToString(),
        .CssClass = "border border-black-solid"
        }

        Dim label_body_za_slovo As New Label() With {
        .ID = "Label_body_za_slovo_" & player_ID.ToString(),
        .Text = body_za_slovo.ToString(),
        .CssClass = "border border-black-solid"
        }

        Dim label_body_spolu As New Label() With {
        .ID = "Label_body_spolu_" & player_ID.ToString(),
        .Text = body_spolu.ToString(),
        .CssClass = "border border-black-solid"
        }

        Dim label_top_body_za_slovo As New Label() With {
        .ID = "Label_top_body_za_slovo_" & player_ID.ToString(),
        .Text = top_body_za_slovo.ToString(),
        .CssClass = "border border-black-solid"
        }

        ' vytvoriť bunky a pridať labels"
        Dim cell_por_cislo As New TableCell()
        cell_por_cislo.Controls.Add(label_por_cislo)
        cell_por_cislo.BorderStyle = BorderStyle.Solid
        cell_por_cislo.BorderWidth = Unit.Pixel(1)

        Dim cell_body_za_slovo As New TableCell()
        cell_body_za_slovo.Controls.Add(label_body_za_slovo)
        cell_body_za_slovo.BorderStyle = BorderStyle.Solid
        cell_body_za_slovo.BorderWidth = Unit.Pixel(1)

        Dim cell_body_spolu As New TableCell()
        cell_body_spolu.Controls.Add(label_body_spolu)
        cell_body_spolu.BorderStyle = BorderStyle.Solid
        cell_body_spolu.BorderWidth = Unit.Pixel(1)

        Dim cell_top_body_za_slovo As New TableCell()
        cell_top_body_za_slovo.Controls.Add(label_top_body_za_slovo)
        cell_top_body_za_slovo.BorderStyle = BorderStyle.Solid
        cell_top_body_za_slovo.BorderWidth = Unit.Pixel(1)

        ' pridať bunky do riadku
        row.Cells.Add(cell_por_cislo)
        row.Cells.Add(cell_PlayerID)
        row.Cells.Add(cell_PlayerName)
        row.Cells.Add(cell_body_za_slovo)
        row.Cells.Add(cell_body_spolu)
        row.Cells.Add(cell_top_body_za_slovo)

        ' pridať riadok do tabuľky
        Table_hraci.Rows.Add(row)
    End Sub


    ''' <summary>
    ''' Definovanie premennej "Body_pocas_hry" - potrebná na zápis do DB tabuľky
    ''' </summary>
    ''' <returns></returns>
    Private Property Body_pocas_hry As String
        Get
            If Session("Body_pocas_hry") IsNot Nothing Then
                Return Session("Body_pocas_hry").ToString()
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal value As String)
            Session("Body_pocas_hry") = value
        End Set
    End Property


    ''' <summary>
    ''' Sub zapíše výsledky hry do DB tabuľky
    ''' </summary>
    ''' <param name="connectionString"></param>
    ''' <param name="query"></param>
    ''' <param name="Elzi_points_for_game"></param>
    ''' <param name="Tomas_points_for_game"></param>
    ''' <param name="Elzi_wins_cumul"></param>
    ''' <param name="Tomas_wins_cumul"></param>
    Public Sub Zapis_do_tabulky_GameCounter_results(Elzi_points_for_game As Integer, Tomas_points_for_game As Integer, Elzi_wins_cumul As Integer, Tomas_wins_cumul As Integer, Rekord_body_za_slovo As Integer, Drzitel_Rekordu_za_slovo As String, Body_pocas_hry As String)

        Dim connectionString As String = "insert connection string here"

        'vyberie sa tabuľka na zapísanie výsledkov hry podľa toho, či sa zvolilo na straánke default.aspx "testovanie" alebo "hra"
        If Session("tabulka") = "test" Then
            query = "INSERT INTO GameCounter_results_test (
                                        date_time,
                                        Elzi_points_for_game,
                                        Tomas_points_for_game,
                                        Elzi_wins_cumul,
                                        Tomas_wins_cumul,
                                        Rekord_body_za_slovo,
                                        Drzitel_rekordu_za_slovo,
                                        Body_pocas_hry) 
                                    VALUES (
                                        UTC_TIMESTAMP(),
                                        @Elzi_points_for_game,
                                        @Tomas_points_for_game,
                                        @Elzi_wins_cumul,
                                        @Tomas_wins_cumul,
                                        @Rekord_body_za_slovo,
                                        @Drzitel_rekordu_za_slovo,
                                        @Body_pocas_hry)"
        Else
            query = "INSERT INTO GameCounter_results (
                                        date_time,
                                        Elzi_points_for_game,
                                        Tomas_points_for_game,
                                        Elzi_wins_cumul,
                                        Tomas_wins_cumul,
                                        Rekord_body_za_slovo,
                                        Drzitel_rekordu_za_slovo,
                                        Body_pocas_hry) 
                                    VALUES (
                                        UTC_TIMESTAMP(),
                                        @Elzi_points_for_game,
                                        @Tomas_points_for_game,
                                        @Elzi_wins_cumul,
                                        @Tomas_wins_cumul,
                                        @Rekord_body_za_slovo,
                                        @Drzitel_rekordu_za_slovo,
                                        @Body_pocas_hry)"
        End If

        'zapísať výsledky hry do príslušnej DB tabuľky
        Using mydbConnection As New MySqlConnection(connectionString)
            Dim myDbCommand As New MySqlCommand(query, mydbConnection)
            myDbCommand.Parameters.AddWithValue("@Elzi_points_for_game", Elzi_points_for_game)
            myDbCommand.Parameters.AddWithValue("@Tomas_points_for_game", Tomas_points_for_game)
            myDbCommand.Parameters.AddWithValue("@Elzi_wins_cumul", Elzi_wins_cumul)
            myDbCommand.Parameters.AddWithValue("@Tomas_wins_cumul", Tomas_wins_cumul)
            myDbCommand.Parameters.AddWithValue("@Rekord_body_za_slovo", Rekord_body_za_slovo)
            myDbCommand.Parameters.AddWithValue("@Drzitel_rekordu_za_slovo", Drzitel_Rekordu_za_slovo)
            myDbCommand.Parameters.AddWithValue("@Body_pocas_hry", Body_pocas_hry)
            myDbCommand.Connection.Open()
            myDbCommand.ExecuteNonQuery()
            myDbCommand.Connection.Close()
        End Using
    End Sub


    Public Sub Zapis_do_tabulky_priebeh_hry(body_za_slovo_elzi As Integer, body_za_slovo_tomas As Integer, body_spolu_elzi As Integer, body_spolu_tomas As Integer)
        Dim connectionString As String = "insert connection string here"
        query = "INSERT INTO Priebeh_hry (
                                        date_time,
                                        body_za_slovo_elzi,
                                        body_za_slovo_tomas,
                                        body_spolu_elzi,
                                        body_spolu_tomas) 
                                    VALUES (
                                        UTC_TIMESTAMP(),
                                        @body_za_slovo_elzi,
                                        @body_za_slovo_tomas,
                                        @body_spolu_elzi,
                                        @body_spolu_tomas)"

        Using mydbConnection As New MySqlConnection(connectionString)
            Dim myDbCommand As New MySqlCommand(query, mydbConnection)
            'myDbCommand.Parameters.AddWithValue("@date_time", Now())
            myDbCommand.Parameters.AddWithValue("@body_za_slovo_elzi", body_za_slovo_elzi)
            myDbCommand.Parameters.AddWithValue("@body_za_slovo_tomas", body_za_slovo_tomas)
            myDbCommand.Parameters.AddWithValue("@body_spolu_elzi", body_spolu_elzi)
            myDbCommand.Parameters.AddWithValue("@body_spolu_tomas", body_spolu_tomas)
            myDbCommand.Connection.Open()
            myDbCommand.ExecuteNonQuery()
            myDbCommand.Connection.Close()
        End Using
    End Sub


    ''' <summary>
    ''' po kliknutí na tlačítko sa z DB tabuľky načíta a zobrazí priebeh aktuálnej hry
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    'Protected Sub Button_prehlad_aktualnej_hry_Click(sender As Object, e As EventArgs)
    '    'Response.Redirect("PriebehHry.aspx?openInNewTab=True", False)
    '    'ClientScript.RegisterStartupScript(Me.GetType(), "OpenNewTab", "window.open('PriebehHry.aspx', '_blank');", True)
    '    Response.Redirect("PriebehHry.aspx")
    'End Sub

End Class