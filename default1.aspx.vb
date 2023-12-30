Imports MySql.Data.MySqlClient
''' <summary>
''' Po spustení aplikácie sa v prehliadači zobrazí úvodná stránka "default.aspx".
''' Stlačením tlačítka s menom hráča, ktorý začína hru, sa zobrazí stránka "competition.aspx".
''' </summary>
Public Class _default
    Inherits System.Web.UI.Page

    Public connectionString As String = "insert connection string here"

    ''' <summary>
    ''' akcie vykonané pri načítavaní stránky
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then

            RadioButtonList1.Items(0).Selected = True
            GenerateDynamicButtons() ' zavolať funkciu na vytvorenie tlačítok
        End If
    End Sub

    ''' <summary>
    ''' akcie vykonané po kliknutí na tlačítko s menom začínajúceho hráča:
    ''' Zobrazí sa stránka "competition.aspx".
    ''' Na nej sa zvolené meno zobrazí v poliach: "Hráč na ťahu:" a "Túto hru začal:".
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub DynamicButtonClick(ByVal sender As Object, ByVal e As EventArgs)

        Dim clickedButton As Button = TryCast(sender, Button)
        ' 2. ODPOVED:
        'If clickedButton IsNot Nothing Then
        '    Dim buttonID As String = clickedButton.ID
        '    Dim playerID As Integer

        '    If buttonID IsNot Nothing AndAlso buttonID.StartsWith("Button_") AndAlso buttonID.EndsWith("_loads_competition_page") Then


        '        Dim idString As String = buttonID.Replace("Button_", "").Replace("_loads_competition_page", "")

        '        If Integer.TryParse(idString, playerID) Then
        '            ' Set the player's name into the session
        '            Session("SelectedPlayerName") = clickedButton.Text

        '            ' For debugging: Output the player's name to check if it's properly set in the session
        '            Response.Write("<script>alert('Player Name in Session: " & Session("SelectedPlayerName").ToString() & "');</script>")

        '            ' Redirect to "competition.aspx" page with playerID parameter
        '            Response.Redirect("competition.aspx?playerID=" & playerID)
        '        End If
        '    End If
        'End If
        '1. ODPOVED:
        'If clickedButton IsNot Nothing Then
        '    Dim buttonID As String = clickedButton.ID
        '    Dim playerID As Integer

        '    If buttonID IsNot Nothing AndAlso buttonID.StartsWith("Button_") AndAlso buttonID.EndsWith("_loads_competition_page") Then
        '        Dim idString As String = buttonID.Replace("Button_", "").Replace("_loads_competition_page", "")

        '        If Integer.TryParse(idString, playerID) Then
        '            ' Set player name in Session variable
        '            Session("SelectedPlayerName") = clickedButton.Text
        '            ' Redirect to "competition.aspx"
        '            Response.Redirect("competition.aspx?playerID=" & playerID)
        '        End If
        '    End If
        'End If
        'ORIGINAL:
        If clickedButton IsNot Nothing Then
            'Dim buttonID As String = clickedButton.ID
            'Dim playerID As Integer

            'If buttonID IsNot Nothing AndAlso buttonID.StartsWith("Button_") AndAlso buttonID.EndsWith("_loads_competition_page") Then
            '    Dim idString As String = buttonID.Replace("Button_", "").Replace("_loads_competition_page", "")

            '    If Integer.TryParse(idString, playerID) Then
            '        ' otvoriť stránku "competition.aspx"

            '        Response.Redirect("competition.aspx?playerID=" & playerID)
            '    End If
            'End If
        End If
    End Sub

    ''' <summary>
    ''' RadioButtonList na voľbu DB tabuľky "test" alebo "hra".
    ''' Tejto voľbe sa potom použije príslušné "query"
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub RadioButtonList1_SelectedIndexChanged(sender As Object, e As EventArgs)
        If RadioButtonList1.SelectedValue = "1" Then
            Session("tabulka") = "test"
        ElseIf RadioButtonList1.SelectedValue = "2" Then
            Session("tabulka") = "hra"
        End If

        ' zavolať funkciu na znovuvytvorenie tlačítok s menami hráčov po zvolení "hra" alebo "test"
        GenerateDynamicButtons()
    End Sub

    ''' <summary>
    ''' funkcia znovu vytvorí tlačítka s menami hráčov, preto6e po zvolení radioButton "Hra" alebo "test" sa prestanú zobrazovať 
    ''' </summary>
    Private Sub GenerateDynamicButtons()

        Dim selectedPlayersList As List(Of SelectedPlayer) = DirectCast(Session("SelectedPlayersList"), List(Of SelectedPlayer))

        For Each selectedPlayer As SelectedPlayer In selectedPlayersList
            ' vytvoriť nové tlačítko
            Dim newButton As New Button()
            newButton.ID = "Button_" & selectedPlayer.PlayerID & "_loads_competition_page"
            newButton.Text = selectedPlayer.PlayerName
            newButton.CssClass = "btn btn-lg btn-info fixed-width-btn text-white mb-2"

            ' priradiť JavaScript funkciu k atribútu "OnClientClick"
            newButton.OnClientClick = "redirectToCompetition(" & selectedPlayer.PlayerID & ", '" & selectedPlayer.PlayerName & "'); return false;"

            ' priradiť tlačítko na placeholder
            dynamicButtonsRow.Controls.Add(newButton)
            dynamicButtonsRow.Controls.Add(New LiteralControl("<br />"))

        Next
        ' "Script block" for "redirection" with "playerName" parameter
        Dim script As String = "<script type='text/javascript'>"
        script &= "function redirectToCompetition(playerID, playerName) {"
        script &= "    window.location.href = 'competition.aspx?playerID=' + playerID + '&playerName=' + encodeURIComponent(playerName);"
        script &= "}"
        script &= "</script>"

        ' Registrovať "script block"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "RedirectScript", script)
    End Sub

    ''' <summary>
    ''' funkcia tlačítka "Zmeniť poradie hráčov" - otvorí stránku "vyberHracov.aspx"
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Button_zmenit_hracov_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("vyberHracov.aspx?clear=true")
    End Sub

End Class

