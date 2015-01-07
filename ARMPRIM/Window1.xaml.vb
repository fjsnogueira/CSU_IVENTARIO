Imports Interop.ErpBS800
Imports System.Data.SqlClient
Imports System.Data
Imports Interop.StdBE800
Imports Interop.GcpBE800
Imports Interop.StdPlatBE800
Imports Interop.StdPlatBS800

Public Class Window1

    Dim motor As ErpBS
    Dim objPlat As StdPlatBS

    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Dim connectionString As String = "Data Source=PRIMAVERASOFT\LE810R2;Initial Catalog= PRIBDARM;User Id= sa;Password=Accsys2011"

    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter

    'Declare the DataSet object
    Dim myDataSet As DataSet

    Public Sub inicializar(ds As DataSet1)
        On Error GoTo trataerro
        dgEntrada.ItemsSource = ds.Tables("Cabec_Doc").DefaultView
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Public Sub Inicializar(erpmotor As ErpBS, erpPlat As StdPlatBS, connection As String)
        On Error GoTo trataerro

        objPlat = erpPlat
        motor = erpmotor
        connectionString = connection

        If motor.Contexto.UtilizadorActual.ToLower = "Solly Asspi".ToLower Or motor.Contexto.UtilizadorActual.ToLower = "ACCSYS".ToLower Then
            btAnular1.IsEnabled = True
            btAnular2.IsEnabled = True
        End If


        myConnection = New SqlConnection(connectionString)

        'Declare the query
        Dim str_query As String = "select distinct(Id),nome, sum(PrecUnit * Quantidade)  as Totaldoc, data from View_Stock_Facturacao_Int where not EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida='S' group by id,nome, data "

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        myConnection.Open()
        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")

        dgEntrada.ItemsSource = ds.Tables("Cabec_Doc").DefaultView

        dpDataInicio2.SelectedDate = DateTime.Now
        dpDataFim2.SelectedDate = DateTime.Now.AddDays(-30)

        dpDataInicio4.SelectedDate = DateTime.Now
        dpDataFim4.SelectedDate = DateTime.Now.AddDays(-30)



        dpDataInicio1.SelectedDate = DateTime.Now
        dpDataFim1.SelectedDate = DateTime.Now.AddDays(-30)

        dpDataInicio3.SelectedDate = DateTime.Now
        dpDataFim3.SelectedDate = DateTime.Now.AddDays(-30)

        chentredatas.IsChecked = False
        chentredatas3.IsChecked = False
        chentredatas2.IsChecked = True
        chentredatas4.IsChecked = True

        actualizar_EntradasStock()
        actualizar_SaidasStock()

        actualizar_ResuldadoEntradas()
        actualizar_ResuldadosSaidas()
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Public Sub actualizar_SaidasStock()

        On Error GoTo trataerro
        'Declare the query
        Dim str_query As String = "select distinct(Id),nome,tipodoc,serie,numdoc, sum(PrecUnit * Quantidade)  as Totaldoc, data from View_Stock_Facturacao_Int where not EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida='S' "

        If chentredatas.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio1.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim1.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,tipodoc,serie,numdoc,data"
        str_query = str_query & " order by tipodoc,serie,numdoc"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)

        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")
        dgEntrada.ItemsSource = ds.Tables("Cabec_Doc").DefaultView
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    'actualizar compras
    Public Sub actualizar_EntradasStock()
        On Error GoTo trataerro

        'Declare the query
        Dim str_query As String = "select distinct(Id),nome,tipodoc,serie,numdoc, sum(PrecUnit * Quantidade)  as Totaldoc, data from View_Stock_Facturacao_Int "
        str_query = str_query + "where not EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida='E' "

        If chentredatas.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio3.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim3.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,tipodoc,serie,numdoc,data"
        str_query = str_query & " order by tipodoc,serie,numdoc"

        myCommand = New SqlCommand(str_query, myConnection)

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")
        dgEntrada2.ItemsSource = ds.Tables("Cabec_Doc").DefaultView

        Exit Sub
trataerro:
        ' MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Public Sub actualizar_ResuldadosSaidas()
        On Error GoTo trataerro

        Dim str_query As String = " select distinct(vs.Id),nome,vs.tipodoc,vs.serie,vs.numdoc, sum(vs.PrecUnit * vs.Quantidade) as Totaldoc, vs.data ," + _
            "cs.tipodoc + '.' + convert(nvarchar,cs.NumDoc)  + '/' + cs.Serie as CabecStock from View_Stock_Facturacao_Int VS inner join CabecSTK cs on vs.Id  = cs.CDU_Idstk "

        str_query = str_query + " where EntradaSaida= 'S' "

        If chentredatas2.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, vs.data, CAST('" & dpDataInicio2.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, vs.data, CAST('" & dpDataFim2.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by vs.id,vs.nome,vs.tipodoc,vs.serie,vs.numdoc,vs.data, cs.CDU_Idstk, cs.TipoDoc,cs.NumDoc,cs.Serie"
        str_query = str_query & " order by vs.tipodoc,vs.serie,vs.numdoc"


        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)

        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")
        dgEntrada_Resultados.ItemsSource = ds.Tables("Cabec_Doc").DefaultView
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Public Sub actualizar_ResuldadoEntradas()

        On Error GoTo trataerro

        ' Dim str_query As String = "select distinct(Id),nome,tipodoc,serie,numdoc, sum(PrecUnit * Quantidade) as Totaldoc, data , (select tipodoc+'.'+ convert(nvarchar,numdoc)+'/'+serie from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) as CabecStock from View_Stock_Facturacao_Int where EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida= 'E' "

        Dim str_query As String = " select distinct(vs.Id),nome,vs.tipodoc,vs.serie,vs.numdoc, sum(vs.PrecUnit * vs.Quantidade) as Totaldoc, vs.data ," + _
            "cs.tipodoc + '.' + convert(nvarchar,cs.NumDoc)  + '/' + cs.Serie as CabecStock from View_Stock_Facturacao_Int VS inner join CabecSTK cs on vs.Id  = cs.CDU_Idstk "

        str_query = str_query + " where EntradaSaida= 'E' "

        If chentredatas4.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, vs.data, CAST('" & dpDataInicio4.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, vs.data, CAST('" & dpDataFim4.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by vs.id,vs.nome,vs.tipodoc,vs.serie,vs.numdoc,vs.data, cs.CDU_Idstk, cs.TipoDoc,cs.NumDoc,cs.Serie"
        str_query = str_query & " order by vs.tipodoc,vs.serie,vs.numdoc"

        myCommand = New SqlCommand(str_query, myConnection)

        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")
        dgEntrada_Resultados2.ItemsSource = ds.Tables("Cabec_Doc").DefaultView
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        actualizar_SaidasStock()
    End Sub

    Private Sub Actualizar_Click(sender As Object, e As RoutedEventArgs)
        actualizar_ResuldadosSaidas()
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        On Error GoTo trataerro
        If (dgEntrada.Items.Count > 0) Then
            Dim i As Integer
            For i = 0 To (dgEntrada.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then

                    Gravadoc(Convert.ToString(selectedFile.Row.ItemArray(0)), "Vendas")

                End If
            Next i
        End If
        MsgBox("Documento Criado com Sucesso")

        actualizar_SaidasStock()

        Exit Sub
trataerro:
        'MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub Gravadoc(id As String, tipo As String)
        On Error GoTo trataerro
        Dim strSQL As String
        Dim objLista As StdBELista
        'Dim impressao As StdPlatBS
        Dim i As Integer
        Dim DocS As GcpBEDocumentoStock

        Dim serie As String
        Dim tipodoc As String
        Dim numdoc As Long
        Dim Empresa As String

        Dim objmotor As New ErpBS

        i = 0
        strSQL = vbNullString
        strSQL = strSQL & "select * from View_Stock_Facturacao_Int where id='" & id & "'"
        objLista = motor.Consulta(strSQL)


        If Not (objLista Is Nothing) Then

            DocS = New GcpBEDocumentoStock

            serie = objLista.Valor("Serie")
            tipodoc = objLista.Valor("TipoDoc")
            numdoc = objLista.Valor("NumDoc")

            Empresa = objLista.Valor("BasedeDados")

            Select Case objLista.Valor("TipoDoc")
                Case "FA", "VD", "ND", "GS", "DI", "DI1", "GS"
                    If objLista.Valor("Modulo") = "V" Then DocS.Tipodoc = "SS"
                Case "NE", "NE1", "GSA"
                    DocS.Tipodoc = "SSA"
                Case "NC", "DV"
                    DocS.Tipodoc = "DS"
                Case "VCA"
                    DocS.Tipodoc = "DCA"
                Case "NCA", "DVA"
                    DocS.Tipodoc = "DSA"
                Case "VD", "VF"
                    If objLista.Valor("Modulo") = "C" Then DocS.Tipodoc = "ES"
                Case "Vc", "VNC"
                    DocS.Tipodoc = "DC"
                Case "VFA"
                    DocS.Tipodoc = "ESA"
                Case "QB" Or "TS" Or "TE" Or "TEA" Or "TSA" Or "SI"
                    DocS.Tipodoc = objLista.Valor("TipoDoc")
            End Select

            DocS.Serie = objLista.Valor("Serie")
            DocS.CamposUtil("CDU_Idstk").Valor = id

            DocS.TipoEntidade = objLista.Valor("TipoEntidade")
            DocS.Entidade = objLista.Valor("Entidade")

            motor.Comercial.Stocks.PreencheDadosRelacionados(DocS)



            While Not (objLista.NoInicio Or objLista.NoFim)

                motor.Comercial.Stocks.AdicionaLinha(DocS, objLista.Valor("Artigo"), objLista.Valor("EntradaSaida"), objLista.Valor("Quantidade"), objLista.Valor("Armazem"), objLista.Valor("PrecUnit"), , , objLista.Valor("Localizacao"))

                'Item seguinte da lista
                objLista.Seguinte()

            End While

            motor.Comercial.Stocks.Actualiza(DocS)

            If tipo = "Vendas" Then

                objmotor.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, Empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)
                objmotor.Comercial.Vendas.ActualizaValorAtributo("000", tipodoc, serie, numdoc, "CDU_Idstk", DocS.Tipodoc + "." + Str(DocS.NumDoc) + "/" + DocS.Serie)
                objmotor.FechaEmpresaTrabalho()

            End If


            If tipo = "Compras" Then

                objmotor.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, Empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)
                objmotor.Comercial.Compras.ActualizaValorAtributo(tipodoc, numdoc, serie, "000", "CDU_Idstk", DocS.Tipodoc + "." + Str(DocS.NumDoc) + "/" + DocS.Serie)
                objmotor.FechaEmpresaTrabalho()

            End If

            If tipo = "Stock" Then


                objmotor.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, Empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)
                objmotor.Comercial.Stocks.ActualizaValorAtributo(tipodoc, numdoc, "000", serie, "CDU_Idstk", DocS.Tipodoc + "." + Str(DocS.NumDoc) + "/" + DocS.Serie)
                objmotor.FechaEmpresaTrabalho()

            End If
        End If

        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub dgEntrada_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgEntrada.SelectionChanged
        On Error GoTo trataerro

        Dim selectedFile As System.Data.DataRowView
        selectedFile = dgEntrada.Items(dgEntrada.SelectedIndex)


        'Declare the query
        Dim str_query As String = "select * from View_Stock_Facturacao_Int where id = '" + selectedFile.Row.ItemArray(0) + "'"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        'myConnection.Open()
        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Linhas_Doc")
        dgLinhasEntrada.ItemsSource = ds.Tables("Linhas_Doc").DefaultView

        Exit Sub
trataerro:
        ' MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub print_Click_1(sender As Object, e As RoutedEventArgs)
        On Error GoTo Erro

        If (dgEntrada_Resultados.Items.Count > 0) Then
            Dim i As Integer
            For i = 0 To (dgEntrada_Resultados.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada_Resultados.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then
                    imprimirDoc(Convert.ToString(selectedFile.Row.ItemArray(0)))
                End If
            Next i
        End If

        Exit Sub
Erro:
        'objPlat = Nothing
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub imprimirDoc(idDoc As String)
        'Dim objPlat As StdPlatBS
        On Error GoTo TrataErro

        Dim strFormula As String
        Dim strSelFormula As String


        objPlat.Mapas.Inicializar("GCP")

        'strFormula = "NumberVar TipoDesc;NumberVar RegimeIva;NumberVar DecQde;NumberVar DecPrecUnit;StringVar MotivoIsencao; TipoDesc:=" & 1 & ";RegimeIva:=3;DecQde:=1;DecPrecUnit:="& 2 & ";MotivoIsencao:=' ';"
        'objPlat.Mapas.AddFormula("InicializaParametros", strFormula)



        strFormula = " StringVar Nome; StringVar Morada;StringVar Localidade; StringVar CodPostal; StringVar Telefone; StringVar Fax; StringVar Contribuinte; StringVar CapitalSocial; StringVar Conservatoria; StringVar Matricula;StringVar MoedaCapitalSocial;StringVar DecQtd;StringVar DecArrMoedaBase;StringVar DecPrecMoedaBase;"

        strFormula = strFormula & "Nome:='" & objPlat.Contexto.Empresa.IDNome & "'"
        strFormula = strFormula & ";Localidade:='" & objPlat.Contexto.Empresa.IDLocalidade & "'"
        strFormula = strFormula & ";CodPostal:='" & objPlat.Contexto.Empresa.IDLocalidadeCod & "'"
        strFormula = strFormula & ";Telefone:='" & objPlat.Contexto.Empresa.IDTelefone & "'"
        strFormula = strFormula & ";Fax:='" & objPlat.Contexto.Empresa.IDFax & "'"
        strFormula = strFormula & ";Contribuinte:='" & objPlat.Contexto.Empresa.IFNIF & "'"
        strFormula = strFormula & ";CapitalSocial:='" & objPlat.Contexto.Empresa.ICCapitalSocial & "'"
        strFormula = strFormula & ";Conservatoria:='" & objPlat.Contexto.Empresa.ICConservatoria & "'"
        strFormula = strFormula & ";Matricula:='" & objPlat.Contexto.Empresa.ICMatricula & "'"
        strFormula = strFormula & ";MoedaCapitalSocial:='" & objPlat.Contexto.Empresa.ICMoedaCapSocial & "'"
        strFormula = strFormula & ";DecQtd:='2';DecArrMoedaBase:='0';DecPrecMoedaBase:='2'"


        strFormula = strFormula & ";"
        objPlat.Mapas.AddFormula("DadosEmpresa", strFormula)

        strSelFormula = "{CabecStk.Filial}='000' And ({CabecStk.TipoDoc}='SS' or {CabecStk.TipoDoc}='SSA' or {CabecStk.TipoDoc}='TS' or {CabecStk.TipoDoc}='TSA' or {CabecStk.TipoDoc}='TE' or {CabecStk.TipoDoc}='TEA' or {CabecStk.TipoDoc}='QB'or {CabecStk.TipoDoc}='QBA' or {CabecStk.TipoDoc}='ES' or {CabecStk.TipoDoc}='ESA' or {CabecStk.TipoDoc}='DSA' or {CabecStk.TipoDoc}='DS' or {CabecStk.TipoDoc}='DCA' or {CabecStk.TipoDoc}='DC') and {CabecStk.Cdu_Idstk}= '" & idDoc & "'"
        objPlat.Mapas.AddFormula("NumVia", "'Original'")
        objPlat.Mapas.SelectionFormula = strSelFormula

        ' Descomentar as duas linhas seguintes para exportar a factura para pdf.
        'objPlat.Mapas.Destino = edFicheiro
        'objPlat.Mapas.SetFileProp efPdf, "TESTE.pdf"
        objPlat.Mapas.ImprimeListagem("GCPARM", "Impressão Documento de Stock", "W", 1, "S", strSelFormula, , , True)
        'objPlat.FechaPlataformaEmpresa()
        'objPlat = Nothing
TrataErro:
        'objPlat = Nothing
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub CheckBox_Checked(sender As Object, e As RoutedEventArgs)
        If chentredatas.IsChecked = True Then
            dpDataInicio1.IsEnabled = True
            dpDataFim1.IsEnabled = True
        Else
            dpDataInicio1.IsEnabled = False
            dpDataFim1.IsEnabled = False
        End If
    End Sub

    Private Sub CheckBox_Click(sender As Object, e As RoutedEventArgs)
        If chentredatas.IsChecked = True Then
            dpDataInicio1.IsEnabled = True
            dpDataFim1.IsEnabled = True
        Else
            dpDataInicio1.IsEnabled = False
            dpDataFim1.IsEnabled = False
        End If
    End Sub

    Private Sub anular_Click_1(sender As Object, e As RoutedEventArgs)

        Dim dv As DataView
        Dim i As Integer
        dv = dgEntrada_Resultados.ItemsSource



        If (dgEntrada_Resultados.Items.Count > 0) Then

            For i = 0 To (dgEntrada_Resultados.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada_Resultados.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then
                    AnulardocSaidas(Convert.ToString(dv.Item(i).Row("Id")), Convert.ToString(dv.Item(i).Row("CabecStock")))
                End If
            Next i

            'MsgBox("OperaDocumento Anulado com Sucesso")

            actualizar_EntradasStock()
        End If

    End Sub

    Private Sub btAnular2_Click_1(sender As Object, e As RoutedEventArgs) Handles btAnular2.Click

        Dim dv As DataView
        Dim i As Integer
        dv = dgEntrada_Resultados2.ItemsSource



        If (dgEntrada_Resultados2.Items.Count > 0) Then

            For i = 0 To (dgEntrada_Resultados2.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada_Resultados2.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then
                    AnulardocSaidas(Convert.ToString(dv.Item(i).Row("Id")), Convert.ToString(dv.Item(i).Row("CabecStock")))
                End If
            Next i

            'MsgBox("OperaDocumento Anulado com Sucesso")

            actualizar_EntradasStock()
        End If

    End Sub

    Private Sub actualizarCompras_Click(sender As Object, e As RoutedEventArgs)
        actualizar_EntradasStock()
    End Sub

    Private Sub ActualizarComprasResultados_Click(sender As Object, e As RoutedEventArgs)
        actualizar_ResuldadoEntradas()
    End Sub

    Private Sub ImportarCompras_Click_1(sender As Object, e As RoutedEventArgs)
        On Error GoTo TrataErro

        If (dgEntrada2.Items.Count > 0) Then
            Dim i As Integer
            For i = 0 To (dgEntrada2.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada2.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then
                    Gravadoc(Convert.ToString(selectedFile.Row.ItemArray(0)), "Compras")
                End If
            Next i

            MsgBox("Documento Criado com Sucesso")

            actualizar_EntradasStock()
        End If

TrataErro:
        'objPlat = Nothing
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    Private Sub printCompras_Click_1(sender As Object, e As RoutedEventArgs)
        On Error GoTo Erro

        If (dgEntrada_Resultados2.Items.Count > 0) Then
            Dim i As Integer
            For i = 0 To (dgEntrada_Resultados2.Items.Count - 1)
                Dim selectedFile As System.Data.DataRowView
                selectedFile = dgEntrada_Resultados2.Items(i)
                If (Convert.ToBoolean(selectedFile.Row.ItemArray(2))) Then
                    imprimirDoc(Convert.ToString(selectedFile.Row.ItemArray(0)))
                End If
            Next i
        End If

        Exit Sub
Erro:
        'objPlat = Nothing
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub dgEntrada2_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgEntrada2.SelectionChanged
        On Error GoTo trataerro

        Dim selectedFile As System.Data.DataRowView
        selectedFile = dgEntrada2.Items(dgEntrada2.SelectedIndex)


        'Declare the query
        Dim str_query As String = "select * from View_Stock_Facturacao_Int where id = '" + selectedFile.Row.ItemArray(0) + "'"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        'myConnection.Open()
        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Linhas_Doc")
        dgLinhasEntrada2.ItemsSource = ds.Tables("Linhas_Doc").DefaultView

        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub

    ''' <summary>
    ''' Anular documentos sincronizados
    ''' </summary>
    ''' <param name="id">id na view de sincronização</param>
    ''' <param name="idStock">id gerado poz sincronização</param>
    ''' <remarks></remarks>

    Private Sub AnulardocSaidas(id As String, idStock As String)
        Dim objLista As StdBELista
        Dim objLista2 As StdBELista

        'Dim impressao As StdPlatBS
        Dim objmotor2 As New ErpBS

        Dim strSQl As String

        Dim objmotor As New ErpBS
        Dim empresa As String
        Dim tipo As String
        Dim continua As Boolean
        continua = False

        strSQl = vbNullString
        strSQl = strSQl & "SELECT * FROM View_Stock_Facturacao_Int where Id='" & id & "'"
        objLista = motor.Consulta(strSQl)

        empresa = vbNullString
        If Not (objLista Is Nothing) Then
            empresa = objLista.Valor("BasedeDados")
            tipo = objLista.Valor("tipo")

            objmotor2.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)

            If tipo = "Stock" Then
                objmotor2.Comercial.Stocks.ActualizaValorAtributo("000", objLista.Valor("TipoDoc"), objLista.Valor("Serie"), objLista.Valor("NumDoc"), "CDU_Idstk", "")
                continua = True
            End If

            If tipo = "Vendas" Then
                objmotor2.Comercial.Vendas.ActualizaValorAtributo("000", objLista.Valor("TipoDoc"), objLista.Valor("Serie"), objLista.Valor("NumDoc"), "CDU_Idstk", "")
                continua = True
            End If

            If tipo = "Compras" Then
                objmotor2.Comercial.Compras.ActualizaValorAtributo(objLista.Valor("TipoDoc"), objLista.Valor("NumDoc"), objLista.Valor("Serie"), "000", "CDU_Idstk", "")
                continua = True
            End If

            If continua = True Then
                strSQl = vbNullString
                strSQl = strSQl & "SELECT * FROM CabecSTK where CDU_Idstk='" & id & "'"
                objLista2 = motor.Consulta(strSQl)

                If Not (objLista2 Is Nothing) Then

                    motor.Comercial.Stocks.Remove(objLista2.Valor("Filial"), "S", objLista2.Valor("TipoDoc"), objLista2.Valor("Serie"), Conversion.Int(objLista2.Valor("NumDoc")))

                End If
            End If
        End If

        If continua = False Then
            MsgBox("Ocorreu um erro durante a transação")
        End If


    End Sub

    Private Sub dgEntrada_SelectionResultadosChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgEntrada_Resultados.SelectionChanged
        On Error GoTo trataerro

        Dim selectedFile As System.Data.DataRowView
        selectedFile = dgEntrada_Resultados.Items(dgEntrada_Resultados.SelectedIndex)


        'Declare the query
        Dim str_query As String = "select distinct(vs.Id), ls.Artigo,ls.PrecUnit,ls.Descricao,ls.Armazem,ls.Quantidade " + _
                        "from View_Stock_Facturacao_Int vs inner join CabecSTK cb on  cb.CDU_Idstk = vs.Id inner " + _
                        "join LinhasSTK ls on ls.IdCabecOrig = cb.Id where vs.id = '" + selectedFile.Row.ItemArray(0) + "'"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        'myConnection.Open()
        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Linhas_Doc")
        dgLinhasResultadosEntrada.ItemsSource = ds.Tables("Linhas_Doc").DefaultView

        Exit Sub
trataerro:
        ' MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub dgEntrada_Resultados2_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgEntrada_Resultados2.SelectionChanged
        On Error GoTo trataerro

        Dim selectedFile As System.Data.DataRowView
        selectedFile = dgEntrada_Resultados2.Items(dgEntrada_Resultados2.SelectedIndex)

        'Declare the query
        Dim str_query As String = "select distinct(vs.Id), ls.Artigo,ls.PrecUnit,ls.Descricao,ls.Armazem,ls.Quantidade " + _
                        "from View_Stock_Facturacao_Int vs inner join CabecSTK cb on  cb.CDU_Idstk = vs.Id inner " + _
                        "join LinhasSTK ls on ls.IdCabecOrig = cb.Id where vs.id = '" + selectedFile.Row.ItemArray(0) + "'"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)
        'myConnection.Open()
        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Linhas_Doc")
        dgLinhasResultadosEntrada2.ItemsSource = ds.Tables("Linhas_Doc").DefaultView

        Exit Sub
trataerro:
        'MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub
End Class
