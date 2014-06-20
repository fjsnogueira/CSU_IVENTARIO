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
    Dim connectionString As String = "Data Source=MAHOTAG\SQLEXPRESS;Initial Catalog= PRIBDARM;User Id= sa;Password=msmz2012!"

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

        chentredatas.IsChecked = True
        chentredatas3.IsChecked = True
        chentredatas2.IsChecked = True
        chentredatas4.IsChecked = True

        actualizar_Compras()
        actualizar_Entradas()

        actualizar_ResuldadosCompras()
        actualizar_ResuldadosSaidas()
        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)
    End Sub



    Public Sub actualizar_Entradas()
        On Error GoTo trataerro
        'Declare the query
        Dim str_query As String = "select distinct(Id),nome, sum(PrecUnit * Quantidade)  as Totaldoc, data from View_Stock_Facturacao_Int where not EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida='S' "

        If chentredatas.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio1.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim1.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,data"

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
    Public Sub actualizar_Compras()
        On Error GoTo trataerro

        'Declare the query
        Dim str_query As String = "select distinct(Id),nome, sum(PrecUnit * Quantidade) as Totaldoc, data from View_Stock_Facturacao_Int where not EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida='E' "

        If chentredatas3.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio3.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim3.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,data"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)

        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
        myAdapter = New SqlDataAdapter(myCommand)
        myAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim ds As New DataSet1()
        myAdapter.Fill(ds, "Cabec_Doc")
        dgEntrada2.ItemsSource = ds.Tables("Cabec_Doc").DefaultView

        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Public Sub actualizar_ResuldadosSaidas()
        On Error GoTo trataerro

        Dim str_query As String = "select distinct(Id),nome, sum(PrecUnit * Quantidade) as Totaldoc, data , (select tipodoc+'.'+ convert(nvarchar,numdoc)+'/'+serie from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) as CabecStock from View_Stock_Facturacao_Int where EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk)  and EntradaSaida= 'S' "

        If chentredatas2.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio2.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim2.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,data"

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

    Public Sub actualizar_ResuldadosCompras()

        On Error GoTo trataerro

        Dim str_query As String = "select distinct(Id),nome, sum(PrecUnit * Quantidade) as Totaldoc, data , (select tipodoc+'.'+ convert(nvarchar,numdoc)+'/'+serie from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) as CabecStock from View_Stock_Facturacao_Int where EXISTS (select cdu_idstk from cabecstk where View_Stock_Facturacao_Int.Id = cdu_idstk) and EntradaSaida= 'E' "

        If chentredatas4.IsChecked = True Then
            str_query = str_query & "and DATEDIFF(day, data, CAST('" & dpDataInicio4.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) >= 0  and  DATEDIFF(day, data, CAST('" & dpDataFim4.SelectedDate.Value.ToString("MM/dd/yyyy") & "' AS DATE) ) <= 0"
        End If

        str_query = str_query & " group by id,nome,data"

        'str_query = "select * from artigo"
        myCommand = New SqlCommand(str_query, myConnection)

        'MessageBox.Show(myCommand.ExecuteScalar().ToString())
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
        actualizar_Entradas()
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

        actualizar_Entradas()

        Exit Sub
trataerro:
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

    End Sub

    Private Sub Gravadoc(id As String, tipo As String)
        On Error GoTo trataerro
        Dim strSQL As String
        Dim objLista As StdBELista
        'Dim impressao As StdPlatBS
        Dim i As Integer
        Dim DocS As GcpBEDocumentoStock

        Dim objmotor As New ErpBS

        i = 0
        strSQL = vbNullString
        strSQL = strSQL & "select * from View_Stock_Facturacao_Int where id='" & id & "'"
        objLista = motor.Consulta(strSQL)

        If tipo = "Vendas" Then
            If Not (objLista Is Nothing) Then

                DocS = New GcpBEDocumentoStock

                DocS.Tipodoc = "SS"

                If (objLista.Valor("TipoDoc") = ("NC") Or objLista.Valor("TipoDoc") = ("DV")) Then
                    DocS.Tipodoc = "DS"
                End If

                If (objLista.Valor("TipoDoc") = ("NE") Or objLista.Valor("TipoDoc") = ("NE1") Or objLista.Valor("TipoDoc") = ("DS1")) Then
                    DocS.Tipodoc = "SSA"
                End If

                If (objLista.Valor("TipoDoc") = ("NC1") Or objLista.Valor("TipoDoc") = ("DV1")) Then
                    DocS.Tipodoc = "DSA"
                End If


                DocS.Serie = objLista.Valor("Serie")
                DocS.CamposUtil("CDU_Idstk").Valor = id

                DocS.TipoEntidade = objLista.Valor("TipoEntidade")
                DocS.Entidade = objLista.Valor("Entidade")

                motor.Comercial.Stocks.PreencheDadosRelacionados(DocS)
                Dim Empresa As String
                Empresa = objLista.Valor("BasedeDados")

                Dim serie As String
                Dim tipodoc As String
                Dim numdoc As Long

                serie = objLista.Valor("Serie")
                tipodoc = objLista.Valor("TipoDoc")
                numdoc = objLista.Valor("NumDoc")

                While Not (objLista.NoInicio Or objLista.NoFim)


                    motor.Comercial.Stocks.AdicionaLinha(DocS, objLista.Valor("Artigo"), "S", objLista.Valor("Quantidade"), objLista.Valor("Armazem"), objLista.Valor("PrecUnit"), , , objLista.Valor("Localizacao"))

                    'Item seguinte da lista
                    objLista.Seguinte()

                End While

                motor.Comercial.Stocks.Actualiza(DocS)

                objmotor.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, Empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)

                objmotor.Comercial.Vendas.ActualizaValorAtributo("000", tipodoc, serie, numdoc, "Cdu_StkId", DocS.ID)

                objmotor.FechaEmpresaTrabalho()

            End If


        Else
            If Not (objLista Is Nothing) Then

                DocS = New GcpBEDocumentoStock
                DocS.Tipodoc = "ES"


                If (objLista.Valor("BasedeDados") = ("BD17") And (objLista.Valor("TipoDoc") = ("VFA") Or objLista.Valor("TipoDoc") = ("VND"))) Then
                    DocS.Tipodoc = "ES"
                End If

                If (objLista.Valor("BasedeDados") = ("BD17") And objLista.Valor("TipoDoc") = ("VNC")) Then
                    DocS.Tipodoc = "DC"
                End If

                If (objLista.Valor("BasedeDados") = ("BD17A") And (objLista.Valor("TipoDoc") = ("VFA") Or objLista.Valor("TipoDoc") = ("VND"))) Then
                    DocS.Tipodoc = "ESA"
                End If

                If (objLista.Valor("BaseDeDados") = ("BD17A") And objLista.Valor("TipoDoc") = ("VNC")) Then
                    DocS.Tipodoc = "DCA"
                End If


                DocS.Serie = objLista.Valor("Serie")
                DocS.CamposUtil("CDU_Idstk").Valor = id

                DocS.TipoEntidade = objLista.Valor("TipoEntidade")
                DocS.Entidade = objLista.Valor("Entidade")

                motor.Comercial.Stocks.PreencheDadosRelacionados(DocS)
                Dim Empresa As String
                Empresa = objLista.Valor("BasedeDados")

                Dim serie As String
                Dim tipodoc As String
                Dim numdoc As Long

                serie = objLista.Valor("Serie")
                tipodoc = objLista.Valor("TipoDoc")
                numdoc = objLista.Valor("NumDoc")

                While Not (objLista.NoInicio Or objLista.NoFim)

                    motor.Comercial.Stocks.AdicionaLinha(DocS, objLista.Valor("Artigo"), "E", objLista.Valor("Quantidade"), objLista.Valor("Armazem"), objLista.Valor("PrecUnit"), , , objLista.Valor("Localizacao"))

                    'Item seguinte da lista
                    objLista.Seguinte()

                End While

                motor.Comercial.Stocks.Actualiza(DocS)


                objmotor.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, Empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)

                objmotor.Comercial.Vendas.ActualizaValorAtributo("000", tipodoc, serie, numdoc, "Cdu_StkId", DocS.ID)

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
        MsgBox("Erro: " & Err.Number & " - " & Err.Description)

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

        strSelFormula = "{CabecStk.Filial}='000' And {CabecStk.TipoDoc}='SS' and {CabecStk.Cdu_Idstk}= '" & idDoc & "'"
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
                    AnulardocSaidas(Convert.ToString(dv.Item(i).Row("Id")), Convert.ToString(dv.Item(i).Row("CabecStock")), "Vendas")
                End If
            Next i

            MsgBox("Documento Criado com Sucesso")

            actualizar_Compras()
        End If
    End Sub

    Private Sub actualizarCompras_Click(sender As Object, e As RoutedEventArgs)
        actualizar_Compras()
    End Sub

    Private Sub ActualizarComprasResultados_Click(sender As Object, e As RoutedEventArgs)
        actualizar_ResuldadosCompras()
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

            actualizar_Compras()
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

    Private Sub AnulardocSaidas(id As String, idStock As String, tipo As String)
        Dim objLista As StdBELista
        Dim objLista2 As StdBELista

        'Dim impressao As StdPlatBS
        Dim objmotor2 As New ErpBS
        
        Dim strSQl As String

        Dim objmotor As New ErpBS
        Dim empresa As String

        strSQl = vbNullString
        strSQl = strSQl & "SELECT * FROM View_Stock_Facturacao_Int where Id='" & id & "'"
        objLista = motor.Consulta(strSQl)

        empresa = vbNullString
        If Not (objLista Is Nothing) Then
            empresa = objLista.Valor("BasedeDados")
            objmotor2.AbreEmpresaTrabalho(motor.Contexto.TipoPlataforma, empresa, motor.Contexto.UtilizadorActual, motor.Contexto.PasswordUtilizadorActual)

            objmotor2.Comercial.Vendas.ActualizaValorAtributo("000", objLista.Valor("TipoDoc"), objLista.Valor("Serie"), objLista.Valor("NumDoc"), "CDU_StkId", "")
        End If

        strSQl = vbNullString
        strSQl = strSQl & "SELECT * FROM CabecSTK where CDU_Idstk='" & id & "'"
        objLista2 = motor.Consulta(strSQl)

        If Not (objLista2 Is Nothing) Then

            motor.Comercial.Stocks.Remove(objLista2.Valor("Filial"), "S", objLista2.Valor("TipoDoc"), objLista2.Valor("Serie"), Conversion.Int(objLista2.Valor("NumDoc")))

        End If

    End Sub

End Class
