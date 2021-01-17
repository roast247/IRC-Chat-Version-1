Imports System.IO
Imports System.Net.Sockets

Public Class Form1
    Dim HasConnection As Boolean = False
    Dim IsTrying As Boolean = False
    Dim MainIRCCLient As TcpClient
    Dim TX As StreamWriter
    Dim RX As StreamReader

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If HasConnection = False Then
            If IsTrying = False Then
                Threading.ThreadPool.QueueUserWorkItem(AddressOf Connect)
            End If
        End If
    End Sub
    Function Connect()
        IsTrying = True
        Try
            MainIRCCLient = New TcpClient
            MainIRCCLient.Connect(TextBoxIP.Text, TextBoxPORT.Text)
            HasConnection = True
            TX = New StreamWriter(MainIRCCLient.GetStream)
            RX = New StreamReader(MainIRCCLient.GetStream)
            Threading.ThreadPool.QueueUserWorkItem(AddressOf Connected)
        Catch ex As Exception
            HasConnection = False
        End Try
        IsTrying = False
        Label2.Text = HasConnection.ToString.ToUpper
        Return True
    End Function
    Function Connected()
        If HasConnection = True Then

            Try
                While MainIRCCLient.Client.Connected = True
                    If RX.BaseStream.CanRead = True Then
                        Dim RawData As String = RX.ReadLine
                        Dim L As Integer = RawData.Length
                        If MainIRCCLient.Client.Connected = False Then
                            Exit While
                        End If
                        RichTextBox1.Text += "<<<" + RawData + vbNewLine
                        If RawData.ToUpper.Contains("@TESTCMD") Then
                            Threading.ThreadPool.QueueUserWorkItem(AddressOf MSGBOX1)
                        End If

                    Else Exit While
                    End If
                End While
                MainIRCCLient.Close()
            Catch ex As Exception
                MainIRCCLient.Close()
            End Try

        End If
        RichTextBox1.Text += ">>>Connection Terminated!" + vbNewLine
        REM Important part
        HasConnection = False
        Try
            ''  Button1.Enabled = True
            Button2.PerformClick()
            '' Button2.Enabled = False
        Catch
        End Try
        Return True
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            MainIRCCLient.Close()
            Label2.Text = HasConnection.ToString.ToUpper
        Catch

        End Try
    End Sub
    Function MSGBOX1()
        MsgBox("TEST!")
        Return True
    End Function
    Private Sub RichTextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles RichTextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Try
                If HasConnection = True Then
                    TX.WriteLine(RichTextBox2.Text)
                    TX.Flush()
                    REM I didn't add the vbnewline but it's important to stop the one line issue.
                    RichTextBox1.Text += ">>><YOU> " + RichTextBox2.Text + vbNewLine
                    RichTextBox2.Clear()
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub

    Private Sub RichTextBox2_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox2.TextChanged

    End Sub

    Private Sub TextBoxIP_TextChanged(sender As Object, e As EventArgs) Handles TextBoxIP.TextChanged

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBoxPORT.TextChanged

    End Sub
End Class
