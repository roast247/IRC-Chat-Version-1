Imports System.IO
Imports System.Net
Imports System.Net.Sockets

Public Class Form1
    Dim ServerPort As Integer = 54000
    Dim ServerStatus As Boolean = False
    Dim ServerTrying As Boolean = False
    Dim Clients As New List(Of TcpClient)
    Dim Server As TcpListener
    Dim Users As New List(Of String)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ServerStatus = False Then
            Server = New TcpListener(IPAddress.Any, ServerPort)
            Server.Start()
            Threading.ThreadPool.QueueUserWorkItem(AddressOf Client_Handler)
            ServerStatus = True
            Button1.Enabled = False
            Button2.Enabled = True
        End If
        Label2.Text = ServerStatus.ToString.ToUpper
    End Sub
    Function Client_Handler(ByVal state As Object)
        Dim TempClient As TcpClient
        Dim NickName As String = "Mendax"
        Dim MyIP As String = ""
        Try
            Dim TX As StreamWriter
            Dim RX As StreamReader
            Using Client As TcpClient = Server.AcceptTcpClient
                If ServerTrying = False Then
                    Threading.ThreadPool.QueueUserWorkItem(AddressOf Client_Handler)
                End If
                MyIP = Client.Client.RemoteEndPoint.ToString
                ListBox1.Items.Add(MyIP)
                If Not Clients.Contains(Client) Then
                    Clients.Add(Client)
                Else Client.Close()
                End If
                TX = New StreamWriter(Client.GetStream)
                RX = New StreamReader(Client.GetStream)
                TempClient = Client
                If Client.Client.Connected = True Then
                    TX.WriteLine("Set your username /Setuser <Nickname>")
                    TX.Flush()
                    TX.WriteLine("No spaces in your nickname. <3")
                    TX.Flush()
                    TX.WriteLine("Until you set a Username, it will show your IP as a nickname.")
                    TX.Flush()

                    While Client.GetStream.CanRead = True
                        If NickName = "Mendax" Then
                            NickName = MyIP
                        End If
                        Dim RawData As String = RX.ReadLine
                        Dim L As Integer = RawData.Length
                        If RawData.StartsWith("/") Then
                            Try
                                If RawData.ToUpper = "/DISCONNECT" Then
                                    Client.Close()
                                ElseIf RawData.ToUpper = "/USERS" Then
                                    Dim UserCount As Integer = 0
                                    TX.WriteLine("User Total(" + Users.Count.ToString + ")")
                                    TX.Flush()
                                    For Each Username1 As String In Users
                                        UserCount += 1
                                        TX.WriteLine("(" + UserCount.ToString + ")" + Username1)
                                        TX.Flush()
                                    Next
                                ElseIf RawData.Contains(" ") Then
                                    Dim Args As String() = RawData.Split(" ")
                                    If Args(0).ToUpper = "/SETUSER" Then
                                        If Users.Contains(NickName) Then
                                            Users.Remove(NickName)
                                        End If
                                        NickName = Args(1)
                                        Users.Add(NickName)
                                        ''   RichTextBox1.Text += "MyIP: " + MyIP + vbNewLine
                                        '' If ListBox1.Items.Contains(MyIP) Then
                                        ''   Console.Beep()
                                        Try
                                            Dim LBB1 As Boolean = False
                                            Dim LBData1 As String = ""
                                            For Each xdata As String In ListBox1.Items
                                                    '' RichTextBox1.Text += "D: " + xdata + vbNewLine
                                                    If xdata.Contains(MyIP) Then
                                                    ''  Console.Beep()
                                                    LBB1 = True
                                                    LBData1 = xdata
                                                    Exit For
                                                    End If
                                                Next
                                            If LBB1 = True Then
                                                Try
                                                    ''  Console.Beep()
                                                    ListBox1.Items.Remove(LBData1)
                                                    LBB1 = False
                                                Catch
                                                End Try
                                            End If
                                        Catch
                                            End Try

                                            ''    End If
                                            ListBox1.Items.Add(MyIP + " " + NickName)
                                            TX.WriteLine("Your new Username is: " + NickName)
                                            TX.Flush()

                                        End If
                                    End If
                            Catch

                            End Try
                        Else
                            SendToAllClients(NickName, RawData, Client.Client.RemoteEndPoint.ToString)
                        End If
                        ConsoleX(NickName, RawData, Client.Client.RemoteEndPoint.ToString)
                    End While
                    If Clients.Contains(TempClient) Then
                        Clients.Remove(TempClient)
                    End If
                    If Users.Contains(NickName) Then
                        Users.Remove(NickName)
                    End If
                    ''  If ListBox1.Items.Contains(MyIP) Then
                    Dim LBB As Boolean = False
                        Dim LBData As String = ""
                        For Each xdata In ListBox1.Items
                            If xdata.ToString.Contains(MyIP) Then
                                LBB = True
                                LBData = xdata
                                Exit For
                            End If
                        Next
                        If LBB = True Then
                            Try
                                ListBox1.Items.Remove(LBData)
                            Catch
                            End Try

                        End If
                    End If
                ''     End If
            End Using
            If Clients.Contains(TempClient) Then
                Clients.Remove(TempClient)
            End If
            If Users.Contains(NickName) Then
                Users.Remove(NickName)
            End If
            Try
                Dim LBB As Boolean = False
                Dim LBData As String = ""
                For Each xdata In ListBox1.Items
                    If xdata.ToString.Contains(MyIP) Then
                        LBB = True
                        LBData = xdata
                        Exit For
                    End If
                Next
                If LBB = True Then
                    Try
                        ListBox1.Items.Remove(LBData)
                    Catch
                    End Try

                End If
            Catch
            End Try
        Catch ex As Exception
            If Clients.Contains(TempClient) Then
                Clients.Remove(TempClient)
            End If
            Try
                TempClient.Close()
            Catch
            End Try
            If Users.Contains(NickName) Then
                Users.Remove(NickName)
            End If
            Try
                Dim LBB As Boolean = False
                Dim LBData As String = ""
                For Each xdata In ListBox1.Items
                    If xdata.ToString.Contains(MyIP) Then
                        LBB = True
                        LBData = xdata
                        Exit For
                    End If
                Next
                If LBB = True Then
                    Try
                        ListBox1.Items.Remove(LBData)
                    Catch
                    End Try
                End If
            Catch
            End Try
        End Try
        Return True
    End Function
    Function ConsoleX(ByVal NickName As String, ByVal Data As String, ByVal IP As String)
        Try
            Dim ConData As String = IP + "<" + NickName + ">" + Data + ";"
            RichTextBox1.Text += ConData + vbNewLine
        Catch
        End Try
        Return True
    End Function

    Function SendToAllClients(ByVal NickName As String, ByVal Data As String, ByVal IP As String)
        Try
            For Each Client As TcpClient In Clients
                If Client.Connected = True Then
                    If Client.Client.Connected = True Then
                        Try
                            If Not Client.Client.RemoteEndPoint.ToString = IP Then
                                Dim ToSendData As String = "<" + NickName + "> " + Data
                                Dim TX As New StreamWriter(Client.GetStream)
                                TX.WriteLine(ToSendData)
                                TX.Flush()
                            End If
                        Catch

                        End Try
                    End If
                End If
            Next
        Catch ex As Exception

        End Try
        Return True
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ServerStatus = True Then
            ServerTrying = True
            Try
                For Each Client As TcpClient In Clients
                    Client.Close()
                Next
                ServerStatus = False
            Catch
                Try
                    Button2.PerformClick()
                Catch
                End Try
            End Try
        End If
        ServerTrying = False
        Button1.Enabled = True
        Button2.Enabled = False
        Label2.Text = ServerStatus.ToString.ToUpper

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub
End Class
