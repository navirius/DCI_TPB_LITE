<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequestTPB.aspx.cs" Inherits="OfficialCeisaLite.Views.RequestTPB" MasterPageFile="~/Site.Master"%>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    <div class="col-md-12">
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
                            <div id="collapseOne" class="panel-collapse collapse">
                                <div class="box-body">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="srcHAWB" class="col-sm-3 control-label">HAWB</label>

                                            <div class="col-sm-9">
                                                <asp:TextBox ID="srcHAWB" runat="server" CssClass="form-control" TextMode="MultiLine" style="height:100px;"></asp:TextBox>                                               
                                            </div>
                                        </div> &nbsp;
                                        <div class="form-group">
                                            <label for="srcHAWB" class="col-sm-3 control-label">Kode Kantor</label>

                                            <div class="col-sm-9">
                                                <asp:DropDownList ID="srcKodeKantor" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                            </div>
                                        </div> &nbsp;
                                        <div class="form-group">
                                            <label for="srcHAWB" class="col-sm-3 control-label">Shipment Status</label>

                                            <div class="col-sm-9">
                                                <asp:DropDownList ID="srcShipmentStatus" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                            </div>
                                        </div> &nbsp;
                                        <div class="form-group">
                                            <label class="col-sm-3 control-label">Total Row</label>

                                            <div class="col-sm-9">
                                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" ID="rblFilterRow" >
                                                    <asp:ListItem Text="100" Value="100" style="margin-right:1em;" Selected="True"/>
                                                    <asp:ListItem Text="200" Value="200" style="margin-right:1em;"/>
                                                    <asp:ListItem Text="500" Value="500" style="margin-right:1em;"/>
                                                    <asp:ListItem Text="1000" Value="1000" />
                                                </asp:RadioButtonList>
                                            </div>
                                        </div> &nbsp;
                                    </div>
                                    <div class="col-md-8">
                                        <div class="form-group">
                                            <label for="srcTglHawb" class="col-sm-3 control-label">Tanggal HAWB</label>

                                            <div class="col-sm-9">
                                                <asp:TextBox ID="srcTglHawb" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>                                               
                                            </div>
                                        </div> &nbsp;
                                        <div class="form-group">
                                            <label for="srcNoAju" class="col-sm-3 control-label">No Aju</label>

                                            <div class="col-sm-9">
                                                <asp:TextBox ID="srcNoAju" runat="server" CssClass="form-control" MaxLength="26"></asp:TextBox> <br />
                                                <asp:CheckBox Text=" &nbsp; Blank No Aju" runat="server" ID="isBlankAju" CssClass="form-check-input"/>
                                            </div>
                                        </div> &nbsp;
                                        <div class="form-group">
                                            <label for="srcStatus" class="col-sm-3 control-label">Status Description</label>

                                            <div class="col-sm-9">
                                                <asp:TextBox ID="srcStatus" runat="server" CssClass="form-control"></asp:TextBox> <br />
                                                <asp:CheckBox Text=" &nbsp; Blank Status Description" runat="server" ID="isBlankStatus" CssClass="form-check-input"/>
                                            </div>
                                        </div>

                                    </div>                                    
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
                    <h3 class="box-title">DCI Data &nbsp<span runat="server" id="totalData" class="label pull-right bg-red"></span></h3>

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
                                    OnPageIndexChanged="GV_DataDCI_PageIndexChanged"
                                    PagerStyle-CssClass="arn-pagination"
                                    CssClass="table box table-hover table-striped table-bordered"
                                    PageSize="15"
                                    DataKeyNames="HAWB, Tgl HAWB, No Aju">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkRowData" runat="server" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="View">                                           
                                            <ItemTemplate> 
                                                <div style="width: 35px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">                                                    
                                                </div>
                                                <asp:LinkButton runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") %>'
                                                    CommandName="ViewRecord"
                                                    CssClass="fa fa-bars fa-2x"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edit">
                                             <ItemTemplate> 
                                                <div style="width: 35px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">                                                    
                                                </div>
                                                <asp:LinkButton runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") %>'
                                                    CommandName="EditBarang"
                                                    CssClass="fa fa-edit fa-2x"
                                                    Style="color:lightseagreen"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delete">
                                             <ItemTemplate> 
                                                <div style="width: 35px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">                                                    
                                                </div>
                                                <asp:LinkButton runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") %>'
                                                    CommandName="DeleteRecord"
                                                    CssClass="fa fa-trash fa-2x"
                                                    Style="color:red"
                                                    OnClientClick="if (!confirm('Are you sure want to delete?')) return false;"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false"/>
                                        <asp:BoundField DataField="Gateway" HeaderText="Gateway" Visible="True"/>
                                        <asp:BoundField DataField="KodeKantor" HeaderText="Kode Kantor" Visible="True"/>
                                        <asp:BoundField DataField="MAWB" HeaderText="MAWB" Visible="True"/>
                                        <asp:TemplateField HeaderText="Tgl MAWB">
                                            <ItemTemplate>
                                                <div style="width: 100px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Tgl MAWB")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="HAWB" HeaderText="HAWB" Visible="True"/>
                                        <asp:TemplateField HeaderText="Tgl HAWB">
                                            <ItemTemplate>
                                                <div style="width: 100px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Tgl HAWB")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="No Aju" HeaderText="No Aju" Visible="True"/>
                                        <asp:TemplateField HeaderText="Nama Penerima">
                                            <ItemTemplate>
                                                <div style="width: 400px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Nama Penerima")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Neg Pengirim">
                                            <ItemTemplate>
                                                <div style="width: 100px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Neg Pengirim")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Type Identitas">
                                            <ItemTemplate>
                                                <div style="width: 100px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Type Identitas")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="No Identitas" HeaderText="No Identitas" Visible="True"/>                                        
                                        <asp:TemplateField HeaderText="Nama Pengirim">
                                            <ItemTemplate>
                                                <div style="width: 400px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Nama Pengirim")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="No BC 1.1">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# DataBinder.GetPropertyValue(Container.DataItem, "No BC 1.1") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tgl BC 1.1">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# DataBinder.GetPropertyValue(Container.DataItem, "Tgl BC 1.1") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="No Pos">
                                            <ItemTemplate>
                                                <div style="width: 50px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("No Pos")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="No Sub Pos">
                                            <ItemTemplate>
                                                <div style="width: 90px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("No Sub Pos")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:BoundField DataField="SPPB" HeaderText="SPPB" Visible="True"/>                                        
                                        <asp:TemplateField HeaderText="Tgl SPPB">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Tgl SPPB")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Nopen" HeaderText="Nopen" Visible="True"/>                                        
                                        <asp:TemplateField HeaderText="Tgl Nopen">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Tgl Nopen")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="FOB" HeaderText="FOB" Visible="True"/>
                                        <asp:BoundField DataField="Freight" HeaderText="Freight" Visible="True"/>
                                        <asp:BoundField DataField="Asuransi" HeaderText="Asuransi" Visible="True"/>
                                        <asp:BoundField DataField="Curr" HeaderText="Curr" Visible="True"/>
                                        <asp:TemplateField HeaderText="Latest Response Time">
                                            <ItemTemplate>
                                                <div style="width: 150px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Latest Response Time")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Latest Response Code">
                                            <ItemTemplate>
                                                <div style="width: 160px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Latest Response Code")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Latest Response Description">
                                            <ItemTemplate>
                                                <div style="width: 150px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Latest Response Description")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Update By">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Update By")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Update Time">
                                            <ItemTemplate>
                                                <div style="width: 150px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# Eval("Update Time")%>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Kirim Data">
                                            <ItemTemplate> 
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">                                                    
                                                </div>
                                                <asp:LinkButton ID="btnSendData" runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") + "," + Eval("No Aju") %>'
                                                    CommandName="SendData"
                                                    Text="Kirim"
                                                    CssClass="btn btn-primary"></asp:LinkButton>                                                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="BC23 Form/Draft">
                                            <ItemTemplate> 
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">                                                    
                                                </div>
                                                <asp:LinkButton ID="btnCreateReport" runat="server"
                                                    CommandArgument='<%#Eval("HAWB") + "," + Eval("Tgl HAWB") %>'
                                                    CommandName="CreateReport"
                                                    Text="Generate"
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
                                <asp:LinkButton ID="btnSendDCI" CssClass="btn btn-primary" runat="server" OnClick="btnSendDCI_Click">Kirim Data</asp:LinkButton>
                                <asp:LinkButton ID="btnSendFinalDCI" CssClass="btn btn-primary" runat="server" OnClick="btnSendFinalDCI_Click" OnClientClick="if (!confirm('Are you sure want to send data FINAL?')) return false;">Kirim Data FINAL</asp:LinkButton>
                                <asp:LinkButton ID="btnRefresh" CssClass="btn btn-primary" runat="server" OnClick="btnRefresh_Click">Download Respon Status</asp:LinkButton>
                                <asp:LinkButton ID="btnCompleteDraft" CssClass="btn btn-warning" runat="server" OnClick="btnCompleteDraft_Click">Complete Draft</asp:LinkButton>
                                <asp:LinkButton ID="btnDownloadDataKPBC" CssClass="btn btn-success" runat="server" OnClick="btnDownloadDataKPBC_Click">Download Data Excel</asp:LinkButton>
                                <%-- hide button --%>
                                <button type="button" id="btnPopupDCI" data-toggle="modal" data-target="#myModalDCI" style="display: none">
                                </button>
                                <button type="button" id="btnPopupSend" data-toggle="modal" data-target="#myModalSend" style="display: none">
                                </button>
                                <button type="button" id="btnPopupBarang" data-toggle="modal" data-target="#myModalBarang" style="display: none">
                                </button>
                                <button type="button" id="btnPopupHistory" data-toggle="modal" data-target="#myModalHistory" style="display: none">
                                </button>
                                <button type="button" id="btnPopupHistoryReject" data-toggle="modal" data-target="#myModalHistoryReject" style="display: none">
                                </button>
                                <button type="button" id="btnPopupLoader" data-toggle="modal" data-target="#myModalLoader" style="display: none">
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

                <asp:UpdateProgress ID="UpdateProgress4" runat="server"  AssociatedUpdatePanelID="UpdatePanelExport">
                    <ProgressTemplate>
                        <asp:Panel ID="Panel7" CssClass="overlay" runat="server">
                            <asp:Panel ID="Panel8" CssClass="loader" runat="server">
                                <i class="fa fa-spinner fa-pulse fa-spin fa-4x"></i>
                                <span class="sr-only">Loading..</span>
                            </asp:Panel>
                        </asp:Panel>
                    </ProgressTemplate>
                </asp:UpdateProgress>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnGetResp" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btnSendFinalDCI" EventName="Click" />
                </Triggers>
                
            </div>
        </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    

    <div class="modal" id="myModalDCI">
        <asp:UpdatePanel ID="UpdatePanelLoad" runat="server" UpdateMode="Always">
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
                                            <asp:TextBox ID="inputHAWB" CssClass="form-control" runat="server" TextMode="MultiLine" style="height:350px; width:200px;"/> <br />
                                            <asp:CheckBox Text=" &nbsp; Re-Load Data DCI" runat="server" ID="isReload" CssClass="form-check-input"/><br />
                                            <asp:CheckBox Text=" &nbsp; Early Draft TPB" runat="server" ID="isDraft" CssClass="form-check-input"/>
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
        <asp:UpdateProgress ID="UpdateProgress2" runat="server"  AssociatedUpdatePanelID="UpdatePanelLoad">
            <ProgressTemplate>
                <asp:Panel ID="Panel3" CssClass="overlay" runat="server">
                    <asp:Panel ID="Panel4" CssClass="loader" runat="server">
                        <i class="fa fa-spinner fa-pulse fa-spin fa-4x"></i>
                        <span class="sr-only">Loading..</span>
                    </asp:Panel>
                </asp:Panel>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSaveHAWB" EventName="Click" />
        </Triggers>
    </div>

    <div class="modal" id="myModalSend">       

        <asp:UpdatePanel ID="UpdatePanelSend" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Form Re-Send Data DCI</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">HAWB & No.AJU</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="listAju" CssClass="form-control" runat="server" TextMode="MultiLine" style="height:100px; width:500px;" ReadOnly="true"/>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-sm-10 control-label" style="color:red"><strong>Nomor aju sudah pernah dikirim ke CEISA 4.0 | Konfirmasi kirim ulang?</strong></label>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseSend" type="button" class="btn btn-danger pull-left" data-dismiss="modal">CANCEL</button>
                            <asp:Button ID="btnSend" runat="server" Text="RESEND" CssClass="btn btn-success" OnClick="btnSend_Click" UseSubmitBehavior="false"/>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress3" runat="server"  AssociatedUpdatePanelID="UpdatePanelSend">
            <ProgressTemplate>
                <asp:Panel ID="Panel5" CssClass="overlay" runat="server">
                    <asp:Panel ID="Panel6" CssClass="loader" runat="server">
                        <i class="fa fa-spinner fa-pulse fa-spin fa-4x"></i>
                        <span class="sr-only">Loading..</span>
                    </asp:Panel>
                </asp:Panel>
            </ProgressTemplate>
        </asp:UpdateProgress>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSend" EventName="Click" />
        </Triggers>
        
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
                            <div class="nav-tabs-custom">
                                <ul class="nav nav-tabs" style="text-align: left;">
                                    <li class="active"><a href="#data_status" data-toggle="tab" id="data_status_tab">Data Status</a></li>
                                    <li><a href="#data_respon" data-toggle="tab" id="data_respon_tab">Data Respon</a></li>
                                </ul>

                                <div class="tab-content">
                                    <div id="data_status" class="tab-pane fade in active">
                                        <div class="table-responsive">                              
                                            <asp:GridView ID="GV_HistoryResponse" runat="server"
                                                AllowPaging="false"
                                                AllowSorting="false"
                                                PagerStyle-CssClass="arn-pagination"
                                                CssClass="table box table-hover table-striped table-bordered">                                        
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <div id="data_respon" class="tab-pane fade in">
                                        <div class="table-responsive">                              
                                            <asp:GridView ID="GV_DataResponse" runat="server"
                                                AllowPaging="false"
                                                AllowSorting="false"
                                                AutoGenerateColumns="false"
                                                OnRowCommand="GV_DataResponse_RowCommand"
                                                PagerStyle-CssClass="arn-pagination"
                                                CssClass="table box table-hover table-striped table-bordered"
                                                DataKeyNames="Action, File Type, AWB, TGL AWB">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-Width="1px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="Action" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="width: 125px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis" >
                                                                <asp:LinkButton ID="btnPrintPDF" runat="server" OnClick="btnPrintPDF_Click">
                                                                    <%# Eval("Action")%>
                                                                </asp:LinkButton>
                                                            </div>
                                                        </ItemTemplate>                                                
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="1px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="File Type" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="width: 100px; overflow: hidden; text-overflow: ellipsis">
                                                                <%# Eval("File Type")%>
                                                            </div>
                                                        </ItemTemplate>                                                
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="1px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="AWB" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="width: 100px; overflow: hidden; text-overflow: ellipsis">
                                                                <%# Eval("AWB")%>
                                                            </div>
                                                        </ItemTemplate>                                                
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="1px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="TGL AWB" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="width: 100px; overflow: hidden; text-overflow: ellipsis">
                                                                <%# Eval("TGL AWB")%>
                                                            </div>
                                                        </ItemTemplate>                                                
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-Width="1px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="Waktu Response" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <div style="width: 200px; overflow: hidden; text-overflow: ellipsis">
                                                                <%# Eval("Response Time")%>
                                                            </div>
                                                        </ItemTemplate>                                                
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
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
                                                AllowPaging="true"
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
                                                OnPageIndexChanging="GV_HistoryRejectNotes_PageIndexChanging"
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

    <div class="modal" id="myModalBarang">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Detail Barang</h4>
                </div>

                <div class="modal-body">
                    <div class="nav-tabs-custom">
                        <ul class="nav nav-tabs" style="text-align: left;">
                            <li class="active"><a href="#barang" data-toggle="tab" id="tab_barang">List Barang</a></li>
                            <li><a href="#dokumen" data-toggle="tab" id="tab_dokumen">List Dokumen</a></li>
                            <li><a href="#entitas" data-toggle="tab" id="tab_entitas">Entitas Importir/Pengusaha TPB</a></li>
                        </ul>

                        <div class="tab-content">
                            <div id="barang" class="tab-pane fade in active">
                                <div class="table-responsive">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-btn">
                                                    <span class="btn btn-primary">
                                                        <i class="fa fa-upload"></i>
                                                        Upload Excel <asp:FileUpload ID="uploadBarang" style="display:none;"  CssClass="form-control"  runat="server"/> 
                                                    </span>
                                
                                                </label>   
                                                <asp:TextBox runat="server" CssClass="form-control" Disabled="true"/>  
                                            </div>
                                        </div>
                                    </div> 
                                    <div class="col-md-2">   
                                        <asp:LinkButton ID="btnSubmitBarang" 
                                            CssClass="btn btn-primary btn-block"
                                            runat="server"
                                            OnClick="btnSubmitBarang_Click">
                                            <i class="fa fa-send"></i> &nbsp;
                                            Submit
                                        </asp:LinkButton>
                                    </div>                                                  
                                    <div class="col-md-6">   
                                        <asp:LinkButton ID="btnDownloadBarang" 
                                            CssClass="btn btn-primary" 
                                            runat="server"
                                            OnClick="btnDownloadBarang_Click">
                                            <i class="fa fa-download"></i>
                                            Download Excel
                                        </asp:LinkButton>
                                    </div>
                                    <asp:UpdatePanel runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <asp:GridView ID="GV_Barang" runat="server"
                                                AllowPaging="true"
                                                AllowSorting="false"
                                                PageSize="10"
                                                PagerStyle-CssClass="arn-pagination"
                                                OnPageIndexChanging="GV_Barang_PageIndexChanging"
                                                CssClass="table box table-hover table-striped table-bordered">                                        
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>                                            
                                </div>
                            </div>
                            
                            <div id="dokumen" class="tab-pane fade in">
                                <div class="table-responsive">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <div class="input-group">
                                                <label class="input-group-btn">
                                                    <span class="btn btn-primary">
                                                        <i class="fa fa-upload"></i>
                                                        Upload Excel <asp:FileUpload ID="uploadDokumen" style="display:none;"  CssClass="form-control"  runat="server"/> 
                                                    </span>
                                
                                                </label>   
                                                <asp:TextBox runat="server" CssClass="form-control" Disabled="true"/>  
                                            </div>
                                        </div>
                                    </div> 
                                    <div class="col-md-2">   
                                        <asp:LinkButton ID="btnSubmitDokumen" 
                                            CssClass="btn btn-primary btn-block"
                                            runat="server"
                                            OnClick="btnSubmitDokumen_Click">
                                            <i class="fa fa-send"></i> &nbsp;
                                            Submit
                                        </asp:LinkButton>
                                    </div>                                                  
                                    <div class="col-md-6">   
                                        <asp:LinkButton ID="btnDownloadDokumen" 
                                            CssClass="btn btn-primary" 
                                            runat="server"
                                            OnClick="btnDownloadDokumen_Click">
                                            <i class="fa fa-download"></i>
                                            Download Excel
                                        </asp:LinkButton>
                                    </div>
                                    <asp:UpdatePanel runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <asp:GridView ID="GV_Dokumen" runat="server"
                                                AllowPaging="true"
                                                AllowSorting="false"
                                                PageSize="5"
                                                PagerStyle-CssClass="arn-pagination"
                                                OnPageIndexChanging="GV_Dokumen_PageIndexChanging"
                                                CssClass="table box table-hover table-striped table-bordered">
                                            </asp:GridView>
                                        </ContentTemplate>
                                </asp:UpdatePanel> 
                                </div>
                            </div>

                            <div id="entitas" class="tab-pane fade in">
                                <div class="form-horizontal">
                                    <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                    
                                            <div class="box-body">
                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">NPWP</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNomorIdentitas" runat="server" CssClass="form-control" MaxLength="15" OnTextChanged="inputNomorIdentitas_TextChanged" AutoPostBack="true" onkeypress="return (event.charCode !=8 && event.charCode ==0 || (event.charCode >= 48 && event.charCode <= 57))"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Nomor SKEP</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNomorIjinEntitas" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                        <asp:DropDownList ID="ddlNomorIjinEntitas" CssClass="form-control select2" runat="server" Visible="false" OnSelectedIndexChanged="ddlNomorIjinEntitas_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Tanggal SKEP</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputTanggalIjinEntitas" runat="server" placeholder="yyyy-mm-dd" CssClass="form-control" MaxLength="10"></asp:TextBox>                        
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Nama</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNamaEntitas" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                </div>                                                

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Alamat</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputAlamatEntitas" runat="server" CssClass="form-control" MaxLength="250" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">NIB</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNibEntitas" runat="server" CssClass="form-control" MaxLength="20" onkeypress="return (event.charCode !=8 && event.charCode ==0 || (event.charCode >= 48 && event.charCode <= 57))"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Jasa Kena Pajak</label>

                                                    <div class="col-sm-9">
                                                        <asp:DropDownList ID="ddlJasaKenaPajak" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="box-footer">
                                                <asp:LinkButton ID="btnSubmitEntitas" 
                                                    CssClass="btn btn-primary pull-right"
                                                    runat="server"
                                                    OnClick="btnSubmitEntitas_Click">
                                                    <i class="fa fa-send"></i> &nbsp;
                                                    Submit
                                                </asp:LinkButton>
                                                <%--<asp:Button ID="btnSubmitEntitas" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSavePrintFP_Click" UseSubmitBehavior="false"/>--%>
                                            </div>
                                        
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>                

                <div class="modal-footer">
                    <button id="btnCloseBarang" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                </div>     
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->            
    </div>
    
    <div class="modal" id="Loader">
        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog modal-sm" style="position:absolute; left:50%; top:25%; transform: translate(-50%, -50%);">
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

                $('.select2').select2();

                //Date picker
                $(<%=srcTglHawb.ClientID%>).datepicker({
                    format: 'yyyy-mm-dd',
                    autoclose: true
                });

                //Date picker
                $(<%=inputTanggalIjinEntitas.ClientID%>).datepicker({
                    format: 'yyyy-mm-dd',
                    autoclose: true
                });

                <%--$('#<%=btnSaveHAWB.ClientID %>').click(function () {

                    var HAWB = $('#<%=inputHAWB.ClientID%>').val();

                    if (HAWB != "") {
                        $('#Loader').modal();
                    }
                });--%>

                <%--$('#<%=btnSend.ClientID %>').click(function () {

                    var HAWB = $('#<%=sendHAWB.ClientID%>').val();

                    if (HAWB != "") {
                        $('#Loader').modal();
                    }
                });--%>

                $('#<%=btnSendDCI.ClientID %>').click(function () {

                    $('#Loader').modal();
                });

                $('#<%=btnRefresh.ClientID %>').click(function () {

                    $('#Loader').modal();
                });

                // This code will attach `fileselect` event to all file inputs on the page
                $(document).on('change', ':file', function () {
                    var input = $(this),
                        numFiles = input.get(0).files ? input.get(0).files.length : 1,
                        label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                    input.trigger('fileselect', [numFiles, label]);
                });


                $(document).ready(function () {
                    //below code executes on file input change and append name in text control
                    $(':file').on('fileselect', function (event, numFiles, label) {

                        var input = $(this).parents('.input-group').find(':text'),
                            log = numFiles > 1 ? numFiles + ' files selected' : label;

                        if (input.length) {
                            input.val(log);
                        } else {
                            if (log) alert(log);
                        }

                    });
                });

                // for bootstrap 3 use 'shown.bs.tab', for bootstrap 2 use 'shown' in the next line
                $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                    // save the latest tab; use cookies if you like 'em better:
                    localStorage.setItem('lastTab', $(this).attr('href'));
                });

                // go to the latest tab, if it exists:
                var lastTab = localStorage.getItem('lastTab');
                if (lastTab) {
                    $('[href="' + lastTab + '"]').tab('show');
                }                
            })
        }

    </script>
</asp:Content>

