Imports Interop.StdPlatBS800
Imports Interop.ErpBS800
Imports Interop.StdBE800

Class MainWindow

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim objmotor As New ErpBS

        Dim objPlat As StdPlatBS
        Dim objConfPlat As StdBSConfApl

        Dim strFormula As String
        Dim strSelFormula As String
        Dim connection As String

        objmotor.AbreEmpresaTrabalho(EnumTipoPlataforma.tpEmpresarial, "BDARM", "accsys", "accsys2011")

        connection = "Data Source=PRIMAVERASOFT\LE810R2;Initial Catalog= PRIBDARM;User Id= sa;Password=Accsys2011"

        objConfPlat = New StdBSConfApl

        objConfPlat.AbvtApl = "GCP"
        objConfPlat.Instancia = "DEFAULT"
        objConfPlat.Utilizador = "accsys"
        objConfPlat.PwdUtilizador = "accsys2011"

        Dim objTrans As StdBETransaccao
        objTrans = New StdBETransaccao
        objPlat = New StdPlatBS
        'objPlat.AbrePlataformaEmpresaIntegrador(objmotor.Contexto.CodEmp, objTrans, objConfPlat, EnumTipoPlataforma.tpProfissional)


        Dim objdll As New ARMPRIM.JanelaPrincipal
        objdll.Inicializar(objmotor, objPlat, connection)
    End Sub
End Class
