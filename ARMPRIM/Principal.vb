Imports System.Data.SqlClient
Imports Interop.ErpBS800
Imports System.Data
Imports Interop.StdPlatBS800

Public Class Principal

End Class

<ComClass(JanelaPrincipal.ClassId, JanelaPrincipal.InterfaceId, JanelaPrincipal.EventsId)>
Public Class JanelaPrincipal
    Dim objMotor As ErpBS
    Public Const ClassId As String = "0DAD61F3-B0B8-4642-A49D-3B37D04BD856"
    Public Const InterfaceId As String = "701B0744-9452-430E-A074-03499961AAD8"
    Public Const EventsId As String = "2FE80C14-6B58-4DAD-AB52-D04ED0646E15"

    'Declare the string variable 'connectionString' to hold the ConnectionString        
    Dim connectionString As String = "Data Source=MAHOTAG\SQLEXPRESS;Initial Catalog= PRIBDARM;User Id= sa;Password=msmz2012!"
    '   "server=MAHOTAG\SQLEXPRESS;" + "User Id=SA;" + "Password=msmz2012!;" +
    '"database=PRIBDARM"



    Dim myConnection As SqlConnection
    Dim myCommand As SqlCommand
    Dim myAdapter As SqlDataAdapter
    'Declare the DataSet object
    Dim myDataSet As DataSet

    Public Sub Inicializar(motor As ErpBS, objPlat As StdPlatBS, connection As String)

        Dim teste As New Window1()
        teste.inicializar(motor, objPlat, connection)
        teste.Show()


    End Sub

    Public Sub New()

    End Sub
End Class