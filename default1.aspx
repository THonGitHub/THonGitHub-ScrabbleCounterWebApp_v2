<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="default1.aspx.vb" Inherits="ScrabbleCounter_webApp._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Game counter</title>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/jquery-ui.css" rel="stylesheet" />
    <link href="CSS.css" rel="stylesheet" />
</head>
<body>   
    <form id="formDefault" runat="server">
        <div class="container" style="background-color: antiquewhite">
            <%-- názov aplikácie --%>
            <div class="row mb-1">
                <div class="col d-flex align-items-center justify-content-center bg-success text-white" style="height: 100px; text-shadow: 2px 2px 7px">
                    <h1>Počítadlo skóre pre hru SCRABBLE</h1>
                </div>                
            </div>

            <%-- Hráči (nápis) a výber DB tabuľky --%>
            <div class="row">
                 <div class="col-2 offset-5 text-center" style="font-size: 30px;">
                     <strong>Hráči:</strong>
                 </div>

                <%-- RadioButtonList: "testovanie" alebo "hra" --%>
                <div class="col-2 offset-3">
                    <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged">
                        <asp:ListItem Text="&nbsp testovanie" Value="1"></asp:ListItem>
                        <asp:ListItem Text="&nbsp hra" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>

            <%--pomocné hlášky --%>
            <div class="row mb-2">
                <div class="col-6 text-end text-white">
                    <asp:Label ID="Label_1sprava" Visible="true" Text="" font-size="12pt" runat="server" BackColor="LightBlue" ForeColor="White"></asp:Label>
                </div>
                <div id="div_2sprava" class="col-6 text-end text-white">
                    <asp:Label ID="Label_2sprava" Visible="true" Text="" font-size="12pt" runat="server" BackColor="lightBlue" ForeColor="White"></asp:Label>
                </div>
            </div>

            <%-- mená hráčov (dynamicky pridávané tlačidlá) --%>
            <div class="row pt-3 bg-secondary-subtle">
                <div class="col text-center">
                    <asp:PlaceHolder ID="dynamicButtonsRow" runat="server"></asp:PlaceHolder>
                </div>
            </div>
            
            <%-- tlačítko "Zmeniť poradie hráčov" --%>
            <div class="row pt-1 pb-2 border-top border-bottom bg-secondary-subtle">
                <div class="col-4 offset-4 text-center">
                    <input id="Button_zmenit_hracov" type="button" value="Zmeniť poradie hráčov" onclick="Button_zmenit_hracov_Click()" />
                </div>                
            </div>
                
            <%-- klikni na meno začínajúceho hráča (nápis) --%>
            <div class="row pt-2">
                <div class="col text-center">
                    Klikni na meno začínajúceho hráča.
                </div>
            </div>        
            
            <%-- obrázok SCRABBLE BOARD (row) --%>
            <div class="row pt-3 pb-3">
                <div class="col text-center">
                    <img class="img-fluid" src="Pictures/scrabble.jpg" alt="scrabble board"/>
                </div>
            </div>
       </div>
    </form>
    <footer>                
        <div class="row text-center">
            <p>© 2023 Game Counter by TomasDiveMan</p>
        </div>
    </footer>
    <script src="Scripts/jquery-3.7.1.js"></script>
    <script src="Scripts/bootstrap.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>  
    <script src="JavaScript.js" defer="defer"></script>
</body>
</html>
