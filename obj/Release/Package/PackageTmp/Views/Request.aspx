<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Request.aspx.cs" Inherits="OfficialCeisaLite.Views.Request" MasterPageFile="~/Site.Master"%>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    <div class="col-md-6">
            <!-- form start -->
            <form id="form1" class="form-horizontal">
                <div class="box-body">
                    <div class="box-group" id="accordion">
                        <div class="panel box box-primary">
                            <div class="box-header with-border">
                                <h4 class="box-title">
                                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseOne">
                                    Filter Data
                                    </a>
                                </h4>
                            </div>
                            <div id="collapseOne" class="panel-collapse collapse in">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="srcHAWB" class="col-sm-3 control-label">HAWB</label>

                                    <div class="col-sm-9">
                                        <asp:TextBox ID="srcHAWB" runat="server" CssClass="form-control" MaxLength="15"></asp:TextBox>                                               
                                    </div>
                                    </div> &nbsp;
                                    <div class="form-group">
                                        <label for="srcTglHawb" class="col-sm-3 control-label">Tanggal HAWB</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcTglHawb" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>                                               
                                        </div>
                                    </div> &nbsp;
                                    <div class="form-group">
                                        <label for="srcNoAju" class="col-sm-3 control-label">No Aju</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcNoAju" runat="server" CssClass="form-control" MaxLength="26"></asp:TextBox>                                               
                                        </div>
                                    </div> &nbsp;
                                </div>
                                <!-- /.box-body -->
                                <div class="box-footer">
                                    <asp:LinkButton ID="btnSearch" CssClass="btn btn-primary pull-right" runat="server" OnClick="btnSearch_Click">Search</asp:LinkButton>               
                                </div>
                                <!-- /.box-footer -->
                            </div>
                        </div>
                    </div>
                </div>
            
            <!-- /.box-body -->
                
            
            </form>
        <!-- /.box -->
    </div>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    

    <asp:UpdatePanel ID="UpdatePanel12" UpdateMode="Always" runat="server">
        <ContentTemplate>
        
        <div class="col-md-12">            
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h3 class="box-title">DCI Data</h3>

                    <div class="box-tools pull-right">
                        <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                        </button>
                    </div>
                </div>

                <div class="box-body">
                    <div class="table-responsive">
                        <asp:UpdatePanel ID="grdUpdatePanel" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:GridView ID="GV_DataDCI" runat="server"
                                    AllowPaging="true"
                                    AllowSorting="false"
                                    AutoGenerateColumns="false"
                                    OnRowCommand="GV_DataDCI_RowCommand"
                                    OnPageIndexChanging="GV_DataDCI_PageIndexChanging"
                                    PagerStyle-CssClass="arn-pagination"
                                    CssClass="table box table-hover table-striped table-bordered">
                                    <Columns>
                                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false"/>
                                        <asp:BoundField DataField="Gateway" HeaderText="Gateway" Visible="True"/>
                                        <asp:BoundField DataField="MAWB" HeaderText="MAWB" Visible="True"/>
                                        <asp:BoundField DataField="Tgl MAWB" HeaderText="Tgl MAWB" Visible="True"/>
                                        <asp:BoundField DataField="HAWB" HeaderText="HAWB" Visible="True"/>
                                        <asp:BoundField DataField="Tgl HAWB" HeaderText="Tgl HAWB" Visible="True"/>
                                        <asp:BoundField DataField="Nama Pengirim" HeaderText="Nama Pengirim" Visible="True"/>
                                        <asp:BoundField DataField="Neg Pengirim" HeaderText="Neg Pengirim" Visible="True"/>
                                        <asp:BoundField DataField="Type Identitas" HeaderText="Type Identitas" Visible="True"/>
                                        <asp:BoundField DataField="No Identitas" HeaderText="No Identitas" Visible="True"/>
                                        <asp:BoundField DataField="Nama Penerima" HeaderText="Nama Penerima" Visible="True"/>
                                        <asp:BoundField DataField="No BC 1.1" HeaderText="No BC 1.1" Visible="True"/>
                                        <asp:BoundField DataField="Tgl BC 1.1" HeaderText="Tgl BC 1.1" Visible="True"/>
                                        <asp:BoundField DataField="No Pos" HeaderText="No Pos" Visible="True"/>
                                        <asp:BoundField DataField="No Sub Pos" HeaderText="No Sub Pos" Visible="True"/>
                                        <asp:BoundField DataField="No Aju" HeaderText="No Aju" Visible="True"/>
                                        <asp:BoundField DataField="SPPB" HeaderText="SPPB" Visible="True"/>
                                        <asp:BoundField DataField="Tgl SPPB" HeaderText="Tgl SPPB" Visible="True"/>
                                        <asp:BoundField DataField="Nopen" HeaderText="Nopen" Visible="True"/>
                                        <asp:BoundField DataField="Tgl Nopen" HeaderText="Tgl Nopen" Visible="True"/>
                                        <asp:BoundField DataField="FOB" HeaderText="FOB" Visible="True"/>
                                        <asp:BoundField DataField="Freight" HeaderText="Freight" Visible="True"/>
                                        <asp:BoundField DataField="Asuransi" HeaderText="Asuransi" Visible="True"/>
                                        <asp:BoundField DataField="Curr" HeaderText="Curr" Visible="True"/>
                                        <asp:BoundField DataField="Latest Response Time" HeaderText="Latest Response Time" Visible="True"/>
                                        <asp:BoundField DataField="Latest Response Code" HeaderText="Latest Response Code" Visible="True"/>
                                        <asp:BoundField DataField="Latest Response Description" HeaderText="Latest Response Description" Visible="True"/>
                                        <asp:BoundField DataField="Update By" HeaderText="Update By" Visible="True"/>
                                        <asp:BoundField DataField="Update Time" HeaderText="Update Time" Visible="True"/>
                                        <asp:TemplateField ItemStyle-Width="125px">
                                            <HeaderTemplate>
                                                <asp:Label Text="Kirim Data" runat="server" />
                                            </HeaderTemplate>
                                            <ItemTemplate>                                                    
                                                <asp:LinkButton ID="btnSendData" runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") + "," + Eval("No Aju") %>'
                                                    CommandName="SendData"
                                                    Text="Kirim"
                                                    CssClass="btn btn-primary"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="125px">
                                            <HeaderTemplate>
                                                <asp:Label Text="Download Response" runat="server" />
                                            </HeaderTemplate>
                                            <ItemTemplate>                                                    
                                                <asp:LinkButton ID="btnGetResp" runat="server"
                                                    CommandArgument='<%#Eval("No Aju") + "," + Eval("HAWB") + "," + Eval("Tgl HAWB")%>'                                                
                                                    Text="Download"
                                                    CssClass="btn btn-primary"
                                                    onclick="btnGetResp_Click"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                            
                                        <asp:TemplateField ItemStyle-Width="125px">
                                            <HeaderTemplate>
                                                <asp:Label Text="Detail History Response" runat="server" />
                                            </HeaderTemplate>
                                            <ItemTemplate>                                                    
                                                <asp:LinkButton ID="btnHistResp" runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") + "," + Eval("Latest Response Code")%>'
                                                    CommandName="ViewHistoryResponse"
                                                    Text="Detail Response"
                                                    CssClass="btn btn-primary"></asp:LinkButton>                                                        
                                            </ItemTemplate>                                
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:updatePanel>
                        <asp:UpdatePanel runat="server" ID="UpdatePanelExport" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:LinkButton ID="btnLoadDCI" CssClass="btn btn-primary" runat="server" OnClick="btnLoadDCI_Click">Load Data DCI</asp:LinkButton>
                                <asp:LinkButton ID="btnSendDCI" CssClass="btn btn-primary" runat="server" OnClick="btnSendDCI_Click">Send Data DCI</asp:LinkButton>
                                <asp:LinkButton ID="btnRefresh" CssClass="btn btn-primary" runat="server" OnClick="btnRefresh_Click">Refresh Data</asp:LinkButton>
                                <%-- hide button --%>
                                <button type="button" id="btnPopupDCI" data-toggle="modal" data-target="#myModalDCI" style="display: none">
                                </button>
                                <button type="button" id="btnPopupSend" data-toggle="modal" data-target="#myModalSend" style="display: none">
                                </button>
                                <button type="button" id="btnPopupHistory" data-toggle="modal" data-target="#myModalHistory" style="display: none">
                                </button>
                                <button type="button" id="btnPopupHistoryReject" data-toggle="modal" data-target="#myModalHistoryReject" style="display: none">
                                </button>
                            </ContentTemplate>
                        </asp:UpdatePanel>                    
                    </div> 
                </div>
                                
                <asp:UpdateProgress ID="UpdateProgress1" runat="server"  AssociatedUpdatePanelID="grdUpdatePanel">
                    <ProgressTemplate>
                        <asp:Panel ID="Panel1" CssClass="overlay" runat="server">
                            <asp:Panel ID="Panel2" CssClass="loader" runat="server">
                                <i class="fa fa-spinner fa-pulse fa-spin fa-4x"></i>
                                <span class="sr-only">Loading..</span>
                            </asp:Panel>
                        </asp:Panel>
                    </ProgressTemplate>
                </asp:UpdateProgress>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnGetResp" EventName="Click" />
                </Triggers>             
            </div>
        </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal" id="myModalDCI">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Form Load Data DCI</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="inputHAWB" class="col-sm-3 control-label">HAWB No.</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="inputHAWB" CssClass="form-control" runat="server" TextMode="MultiLine" style="height:350px; width:200px;"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseHAWB" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                            <asp:Button ID="btnSaveHAWB" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSaveHAWB_Click" UseSubmitBehavior="false"/>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="modal" id="myModalSend">       

        <asp:UpdatePanel ID="UpdatePanelSend" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Form Send Data DCI</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="sendHAWB" class="col-sm-3 control-label">HAWB No.</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="sendHAWB" CssClass="form-control" runat="server" TextMode="MultiLine" style="height:350px; width:200px;"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseSend" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                            <asp:Button ID="btnSend" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSend_Click" UseSubmitBehavior="false"/>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>            
        
    </div>

    <div class="modal" id="myModalHistory">
        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">History Response</h4>
                        </div>
                        <div class="modal-body">
                            <div class="table-responsive">                              
                                <asp:GridView ID="GV_HistoryResponse" runat="server"
                                    AllowPaging="false"
                                    AllowSorting="false"
                                    PagerStyle-CssClass="arn-pagination"
                                    CssClass="table box table-hover table-striped table-bordered">                                        
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseHistory" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="modal" id="myModalHistoryReject">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">History Response</h4>
                        </div>
                        <div class="modal-body">
                            <div class="nav-tabs-custom">
                                <ul class="nav nav-tabs" style="text-align: left;">
                                    <li class="active"><a href="#first" data-toggle="tab" id="first_tab">Reject Status</a></li>
                                    <li><a href="#second" data-toggle="tab" id="second_tab">Reject Notes</a></li>
                                </ul>

                                <div class="tab-content">
                                    <div id="first" class="tab-pane fade in active">
                                        <div class="table-responsive">                              
                                            <asp:GridView ID="GV_HistoryReject" runat="server"
                                                AllowPaging="false"
                                                AllowSorting="false"
                                                PagerStyle-CssClass="arn-pagination"
                                                CssClass="table box table-hover table-striped table-bordered">                                        
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div id="second" class="tab-pane fade in">
                                        <div class="table-responsive">                              
                                            <asp:GridView ID="GV_HistoryRejectNotes" runat="server"
                                                AllowPaging="true"
                                                AllowSorting="false"
                                                PagerStyle-CssClass="arn-pagination"
                                                CssClass="table box table-hover table-striped table-bordered">
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseHistoryReject" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="modal" id="Loader">
        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog modal-sm modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-body" style="height:70px;">
                            <div class="col-md-3">
                                <i class="fa fa-spinner fa-3x fa-spin"></i>
                            </div>
                            <div class="col-md-9">
                                <h4>Processing Data..</h4>
                            </div>
                            <%--hidden button--%>
                            <button id="btnCloseLoader" type="button" class="btn btn-default pull-left" data-dismiss="modal" style="display: none">Close</button>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <script>
        function pageLoad() {
            $(function () {
                //Date picker
                $(<%=srcTglHawb.ClientID%>).datepicker({
                    format: 'yyyy-mm-dd',
                    autoclose: true
                });

                $('#<%=btnSaveHAWB.ClientID %>').click(function () {

                    var HAWB = $('#<%=inputHAWB.ClientID%>').val();

                    if (HAWB != "") {
                        $('#Loader').modal();
                    }
                });

                $('#<%=btnSend.ClientID %>').click(function () {

                    var HAWB = $('#<%=sendHAWB.ClientID%>').val();

                    if (HAWB != "") {
                        $('#Loader').modal();
                    }
                });
            })
        }
    </script>
</asp:Content>

