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

    <style>
        .btn-spinner {
            display: inline-block;
            width: 14px; height: 14px;
            border: 2px solid rgba(255,255,255,0.35);
            border-top-color: #fff;
            border-radius: 50%;
            animation: btnSpin 0.7s linear infinite;
            vertical-align: middle;
            margin-right: 4px;
        }
        @keyframes btnSpin { to { transform: rotate(360deg); } }

        .loading-overlay {
            display: none;
            position: absolute;
            inset: 0;
            background: rgba(255,255,255,0.75);
            z-index: 100;
            align-items: center;
            justify-content: center;
            flex-direction: column;
            gap: 10px;
            border-radius: 4px;
        }
        .loading-overlay.active { display: flex; }
        .loading-overlay .overlay-spinner {
            width: 32px; height: 32px;
            border: 3px solid #d0dae8;
            border-top-color: #337ab7;
            border-radius: 50%;
            animation: btnSpin 0.8s linear infinite;
        }
        .loading-overlay .overlay-text {
            font-size: 13px; color: #555;
        }
        /* Success Popup */
        .success-backdrop {
            display: flex;
            position: fixed;
            top: 0; left: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(0,0,0,0.35);
            z-index: 99999;              /* lebih tinggi dari modal Bootstrap (z-index: 1050) */
            align-items: center;
            justify-content: center;
            visibility: hidden;
            opacity: 0;
            pointer-events: none;
            transition: opacity 0.2s ease, visibility 0.2s ease;
        }
        .success-backdrop.show { 
            visibility: visible;
            opacity: 1;
            pointer-events: auto;
        }
        @keyframes spFadeIn { from { opacity: 0; } to { opacity: 1; } }

        .success-popup {
            position: relative;          /* bukan fixed/absolute */
            background: #fff;
            border-radius: 12px;
            padding: 2rem 2.5rem;
            text-align: center;
            width: 320px;
            margin: auto;                /* fallback centering */
        }
        @keyframes spPopIn {
          from { transform: scale(0.8); opacity: 0; }
          to   { transform: scale(1);   opacity: 1; }
        }
        .success-icon-wrap {
            width: 64px; height: 64px; border-radius: 50%;
            background: #eaf3de;
            display: flex; align-items: center; justify-content: center;
            margin: 0 auto 1rem;
        }
        .success-checkmark {
            width: 32px; height: 32px;
            stroke: #3B6D11; stroke-width: 3;
            stroke-linecap: round; stroke-linejoin: round; fill: none;
            stroke-dasharray: 50; stroke-dashoffset: 50;
            animation: spDrawCheck 0.4s ease 0.15s forwards;
        }
        @keyframes spDrawCheck { to { stroke-dashoffset: 0; } }
        .success-title { font-size: 17px; font-weight: 500; margin: 0 0 6px; color: #222; }
        .success-msg   { font-size: 13px; color: #666; margin: 0 0 1.5rem; line-height: 1.6; }
        .btn-ok {
          padding: 8px 32px; background: #337ab7; color: #fff;
          border: none; border-radius: 6px;
          font-size: 14px; font-weight: 500; cursor: pointer;
        }
        .btn-ok:hover { background: #2368a2; }

        /* --- FIX JQUERY UI AUTOCOMPLETE DI DALAM MODAL --- */
        .ui-autocomplete {
            background: #ffffff !important; /* Menimpa error 404 gambar background hilang */
            border: 1px solid #ccc;
            z-index: 99999 !important; /* Memastikan dropdown tidak tertutup Modal */
            max-height: 200px;
            overflow-y: auto;
            overflow-x: hidden;
            padding: 0;
            margin: 0;
            list-style: none;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);

            top: 100% !important; /* Paksa turun tepat ke bawah TextBox */
            left: 15px !important; /* Kompensasi padding kiri bawaan Bootstrap col-sm-9 */
            width: calc(100% - 30px) !important; /* Samakan lebarnya dengan TextBox */
        }
        .ui-autocomplete .ui-menu-item {
            padding: 8px 10px;
            cursor: pointer;
            border-bottom: 1px solid #eee;
            font-size: 13px;
        }
        .ui-autocomplete .ui-state-active, 
        .ui-autocomplete .ui-state-focus {
            background: #337ab7 !important;
            color: white !important;
            border: none !important;
            margin: 0 !important;
        }
    </style>

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
                                    OnRowDataBound="GV_DataDCI_RowDataBound"
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
                                        <asp:TemplateField HeaderText="Profect Freight">
                                            <ItemTemplate>
                                                <asp:Label ID="lblProfectFreight" runat="server" 
                                                           Text='<%# Eval("Profect Freight Rp") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Nilai Currency" HeaderText="Nilai Currency" Visible="True"/>
                                        <asp:BoundField DataField="FOB" HeaderText="FOB" Visible="True"/>
                                        <asp:BoundField DataField="Freight" HeaderText="Freight" Visible="True"/>
                                        <asp:BoundField DataField="Asuransi" HeaderText="Asuransi" Visible="True"/>
                                        <asp:BoundField DataField="Curr" HeaderText="Curr" Visible="True"/>
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

    <div class="modal" id="myModalBarang" data-backdrop="static" data-keyboard="false">
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
                            <li><a href="#kemasan" data-toggle="tab" id="tab_kemasan">Kemasan</a></li>
                            <li><a href="#entitas" data-toggle="tab" id="tab_entitas">Entitas</a></li>
                            <li><a href="#transaksi" data-toggle="tab" id="tab_transaksi">Transaksi & Pelabuhan</a></li>
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
                                            OnClick="btnSubmitBarang_Click"
                                            OnClientClick="setButtonLoading(this, 'Mengupload...');">
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
                                <div class="table-responsive" style="overflow-x: hidden;">
                                    <div class="row" style="margin-bottom: 15px; margin-top: 10px;">
                                        <div class="col-md-4">
                                            <div class="form-group" style="margin-bottom: 0;">
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
                                                OnClick="btnSubmitDokumen_Click"
                                                OnClientClick="setButtonLoading(this, 'Mengupload...');">
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
                                    </div>
                                    
                                    <hr style="margin-top: 10px; margin-bottom: 15px; border-top: 1px solid #ddd;" />

                                    <asp:UpdatePanel runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <div class="row" style="margin-bottom: 15px;">
                                                <div class="col-md-3">
                                                    <asp:TextBox ID="txtNewKodeDokumen" runat="server" CssClass="form-control" Placeholder="Kode (cth: 380)"></asp:TextBox>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtNewNomorDokumen" runat="server" CssClass="form-control" Placeholder="Nomor Dokumen"></asp:TextBox>
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:TextBox ID="txtNewTanggalDokumen" runat="server" CssClass="form-control" Placeholder="Tgl (yyyy-mm-dd)"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2">
                                                    <asp:LinkButton ID="btnTambahDokumenManual" runat="server" CssClass="btn btn-success btn-block" OnClick="btnTambahDokumenManual_Click">
                                                        <i class="fa fa-plus"></i> Tambah
                                                    </asp:LinkButton>
                                                </div>
                                            </div>

                                            <asp:GridView ID="GV_Dokumen" runat="server"
                                                AllowPaging="true"
                                                AllowSorting="false"
                                                PageSize="5"
                                                PagerStyle-CssClass="arn-pagination"
                                                OnPageIndexChanging="GV_Dokumen_PageIndexChanging"
                                                OnRowEditing="GV_Dokumen_RowEditing"
                                                OnRowCancelingEdit="GV_Dokumen_RowCancelingEdit"
                                                OnRowUpdating="GV_Dokumen_RowUpdating"
                                                OnRowDeleting="GV_Dokumen_RowDeleting"
                                                AutoGenerateColumns="false"
                                                DataKeyNames="HAWB,TGL_HAWB,seriDokumen"
                                                CssClass="table box table-hover table-striped table-bordered">
                                                <Columns>
                                                    <asp:CommandField ShowEditButton="True" 
                                                        ControlStyle-CssClass="btn btn-xs btn-primary" 
                                                        CancelText="Batal" EditText="Edit" UpdateText="Simpan" />

                                                    <asp:TemplateField HeaderText="Aksi">
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server" CommandName="Delete" CssClass="btn btn-xs btn-danger"
                                                                OnClientClick="if (!confirm('Apakah Anda yakin ingin menghapus dokumen ini?')) return false;">
                                                                <i class="fa fa-trash"></i> Hapus
                                                            </asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="kodeDokumen" HeaderText="Kode Dokumen" ControlStyle-CssClass="form-control input-sm" />
                                                    <asp:BoundField DataField="nomorDokumen" HeaderText="Nomor Dokumen" ControlStyle-CssClass="form-control input-sm" />
                                                    <asp:BoundField DataField="tanggalDokumen" HeaderText="Tgl Dokumen (yyyy-mm-dd)" ControlStyle-CssClass="form-control input-sm" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                </asp:UpdatePanel> 
                                </div>
                            </div>

                            <div id="kemasan" class="tab-pane fade in">
                                <div class="table-responsive" style="overflow-x: hidden; padding: 15px;">
                                    <asp:UpdatePanel runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                
                                            <asp:GridView ID="GV_Kemasan" runat="server"
                                                AllowPaging="true"
                                                AllowSorting="false"
                                                PageSize="5"
                                                PagerStyle-CssClass="arn-pagination"
                                                OnPageIndexChanging="GV_Kemasan_PageIndexChanging"
                                                OnRowEditing="GV_Kemasan_RowEditing"
                                                OnRowCancelingEdit="GV_Kemasan_RowCancelingEdit"
                                                OnRowUpdating="GV_Kemasan_RowUpdating"
                                                AutoGenerateColumns="false"
                                                DataKeyNames="HAWB,TGL_HAWB,seriKemasan"
                                                CssClass="table box table-hover table-striped table-bordered">
                                                <Columns>
                                                    <asp:CommandField ShowEditButton="True" 
                                                        ControlStyle-CssClass="btn btn-xs btn-primary" 
                                                        CancelText="Batal" EditText="Edit" UpdateText="Simpan" />
                        
                                                    <asp:BoundField DataField="seriKemasan" HeaderText="Seri" ReadOnly="true" ControlStyle-CssClass="form-control input-sm" />
                        
                                                    <asp:BoundField DataField="jumlahKemasan" HeaderText="Jumlah Kemasan" ControlStyle-CssClass="form-control input-sm" />
                                                    <asp:BoundField DataField="kodeJenisKemasan" HeaderText="Kode Jenis (cth: PK)" ControlStyle-CssClass="form-control input-sm" />
                                                    <asp:BoundField DataField="merkKemasan" HeaderText="Merk" ControlStyle-CssClass="form-control input-sm" />
                                                </Columns>
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
                                                <h3 style="text-align:center; padding-bottom:5px;">Importir/Pengusaha TPB</h3>
                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">NPWP</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNomorIdentitas" runat="server" CssClass="form-control" MaxLength="22" OnTextChanged="inputNomorIdentitas_TextChanged" AutoPostBack="true" onkeypress="return (event.charCode !=8 && event.charCode ==0 || (event.charCode >= 48 && event.charCode <= 57))"></asp:TextBox>
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
                                                    <label class="col-sm-3 control-label">Nama Importir</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNamaEntitas" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                </div>                                                

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Alamat Importir</label>

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
                                                <hr />
                                                <h3 style="text-align:center; padding-bottom:5px;">Pemasok</h3>
                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Nama Pemasok</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputNamaPemasokEntitas" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                </div>                                                

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Alamat Pemasok</label>

                                                    <div class="col-sm-9">
                                                        <asp:TextBox ID="inputAlamatPemasokEntitas" runat="server" CssClass="form-control" MaxLength="250" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Negara</label>

                                                    <div class="col-sm-9" id="wrapNegara" style="position: relative;">
        
                                                        <asp:TextBox ClientIDMode="Static" ID="inputNegaraIdentitas" runat="server" CssClass="form-control" Placeholder="Ketik min. 2 huruf (cth: ID)..."></asp:TextBox>
                                                        <asp:HiddenField ClientIDMode="Static" ID="hfNegaraIdentitas" runat="server" />
                                                        <small class="text-muted" style="font-style:italic;">*Ketik minimal 2 karakter kode atau nama negara.</small>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="box-footer">
                                                <asp:LinkButton ID="btnSubmitEntitas" 
                                                    CssClass="btn btn-primary pull-right"
                                                    runat="server"
                                                    OnClick="btnSubmitEntitas_Click"
                                                    OnClientClick="setButtonLoading(this, 'Menyimpan...');">
                                                    <i class="fa fa-send"></i> &nbsp;
                                                    Submit
                                                </asp:LinkButton>
                                                <%--<asp:Button ID="btnSubmitEntitas" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSavePrintFP_Click" UseSubmitBehavior="false"/>--%>
                                            </div>
                                        
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>

                            <div id="transaksi" class="tab-pane fade in">
                                <div class="form-horizontal">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
        
                                            <div class="box-body">
                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Jenis Valuta</label>

                                                    <div class="col-sm-3">
                                                        <asp:DropDownList ClientIDMode="Static" ID="ddlJenisValuta" runat="server" CssClass="form-control select2"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <asp:TextBox ClientIDMode="Static" ID="txtNdpbm" runat="server" CssClass="form-control" MaxLength="20" onkeypress="return (event.charCode !=8 && event.charCode ==0 || (event.charCode >= 48 && event.charCode <= 57))"></asp:TextBox>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div style="display: flex; align-items: center;">
                                                            <asp:LinkButton ID="btnReloadKurs" runat="server" CssClass="btn-refresh" 
                                                                OnClick="btnReloadKurs_Click" style="padding: 8px 12px; display: inline-block;">
                                                                <i class="fa fa-refresh"></i>
                                                            </asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Incoterm</label>

                                                    <div class="col-sm-9">
                                                        <asp:DropDownList ClientIDMode="Static" ID="ddlKodeIncoterm" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label class="col-sm-3 control-label">Pelabuhan Muat</label>

                                                    <div class="col-sm-9" id="wrapPelabuhanMuat" style="position: relative;">
        
                                                        <asp:TextBox ClientIDMode="Static" ID="txtPelabuhanMuat" runat="server" CssClass="form-control" Placeholder="Ketik min. 2 huruf (cth: AMQ)..."></asp:TextBox>
                                                        <asp:HiddenField ClientIDMode="Static" ID="hfPelabuhanMuat" runat="server" />
                                                        <small class="text-muted" style="font-style:italic;">*Ketik minimal 2 karakter kode atau nama pelabuhan.</small>
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="box-footer">
                                                <asp:LinkButton ID="btnSubmitTransaksi" 
                                                    CssClass="btn btn-primary pull-right"
                                                    runat="server"
                                                    OnClick="btnSubmitTransaksi_Click"
                                                    OnClientClick="setButtonLoading(this, 'Menyimpan...');">
                                                    <i class="fa fa-send"></i> &nbsp;
                                                    Submit
                                                </asp:LinkButton>                                                
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

    <%--<div class="success-backdrop" id="successBackdrop">
        <div class="success-popup">
            <div class="success-icon-wrap">
                <svg class="success-checkmark" viewBox="0 0 24 24">
                    <polyline points="4,13 9,18 20,6"/>
                </svg>
            </div>
            <div class="success-title">Berhasil!</div>
            <div class="success-msg" id="successMsgText">Data berhasil disimpan.</div>
            <button class="btn-ok" onclick="closeSuccessPopup(event)">OK</button>
        </div>
    </div>--%>
    
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

                $('#<%=btnSendDCI.ClientID %>').click(function (e) {
                    if ($(this).hasClass('disabled')) {
                        e.preventDefault();
                        return false;
                    }
                    $(this).addClass('disabled'); // Disable klik selanjutnya
                    $('#Loader').modal({ backdrop: 'static', keyboard: false }); // Tampilkan loader
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

                // --- TAMBAHAN UNTUK PELABUHAN ---
                // cek dulu apakah textboxnya sedang dirender di layar
                if ($("#txtPelabuhanMuat").length > 0) {
                    $("#txtPelabuhanMuat").autocomplete({
                        source: "RequestTPB.aspx?action=getpelabuhan",
                        minLength: 2,
                        // KUNCI FIX ERROR getClientRects: paksa nempel ke modal, bukan ke window/body
                        appendTo: "#wrapPelabuhanMuat",
                        select: function (event, ui) {
                            $('#txtPelabuhanMuat').val(ui.item.label);
                            $('#hfPelabuhanMuat').val(ui.item.value);
                            return false;
                        }
                    });
                }
                // ---------------------------------------

                // --- TAMBAHAN UNTUK NEGARA ---
                if ($("#inputNegaraIdentitas").length > 0) {
                    $("#inputNegaraIdentitas").autocomplete({
                        source: "RequestTPB.aspx?action=getnegara",
                        minLength: 2,
                        // KUNCI FIX ERROR getClientRects: paksa nempel ke modal, bukan ke window/body                        
                        appendTo: "#wrapNegara",
                        select: function (event, ui) {
                            $('#inputNegaraIdentitas').val(ui.item.label);
                            $('#hfNegaraIdentitas').val(ui.item.value);
                            return false;
                        }
                    });
                }
                // ---------------------------------------
            });
        }

        // Tampilkan loading pada button
        function setButtonLoading(btn, loadingText) {
            btn.disabled = true;
            btn.setAttribute('data-original-html', btn.innerHTML);
            btn.innerHTML = '<span class="btn-spinner"></span> ' + (loadingText || 'Memproses...');
        }

        // Kembalikan button ke semula (dipanggil setelah proses selesai)
        function resetButton(btn) {
            btn.disabled = false;
            btn.innerHTML = btn.getAttribute('data-original-html');
        }

        // Tampilkan overlay pada container tertentu
        function showOverlay(containerId, text) {
            var container = document.getElementById(containerId);
            if (!container) return;
            container.style.position = 'relative';
            var overlay = document.createElement('div');
            overlay.className = 'loading-overlay active';
            overlay.id = containerId + '_overlay';
            overlay.innerHTML = '<div class="overlay-spinner"></div>'
                + '<div class="overlay-text">' + (text || 'Sedang memproses...') + '</div>';
            container.appendChild(overlay);
        }

        function hideOverlay(containerId) {
            var el = document.getElementById(containerId + '_overlay');
            if (el) el.remove();
        }
        function showSuccessPopup(msg) {
            // Hapus popup lama jika ada
            var existing = document.getElementById('successBackdrop');
            if (existing) existing.remove();

            // Buat popup langsung di body, di luar modal
            var backdrop = document.createElement('div');
            backdrop.id = 'successBackdrop';
            backdrop.className = 'success-backdrop show';
            backdrop.innerHTML =
                '<div class="success-popup" onclick="event.stopPropagation();">' +
                '<div class="success-icon-wrap">' +
                '<svg class="success-checkmark" viewBox="0 0 24 24">' +
                '<polyline points="4,13 9,18 20,6"/>' +
                '</svg>' +
                '</div>' +
                '<div class="success-title">Berhasil!</div>' +
                '<div class="success-msg">' + (msg || 'Data berhasil disimpan.') + '</div>' +
                '<button class="btn-ok" onclick="closeSuccessPopup(event)">OK</button>' +
                '</div>';

            // Append langsung ke body, bukan ke dalam modal
            document.body.appendChild(backdrop);

            backdrop.onclick = function (e) {
                e.stopPropagation();
                if (e.target === backdrop) closeSuccessPopup(e);
            };
        }

        function closeSuccessPopup(event) {
            if (event) {
                event.stopPropagation();
                event.preventDefault();
            }
            var backdrop = document.getElementById('successBackdrop');
            if (backdrop) backdrop.remove();
        }

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                // Kalau popup sukses masih buka, Escape hanya tutup popup, bukan modal
                var backdrop = document.getElementById('successBackdrop');
                if (backdrop) {
                    e.stopImmediatePropagation();
                    closeSuccessPopup();
                }
            }
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // Sembunyikan semua overlay jika masih ada
            document.querySelectorAll('.loading-overlay').forEach(function (el) { el.remove(); });
            // Reset semua button yang masih disabled
            document.querySelectorAll('a[disabled]').forEach(function (btn) {
                btn.removeAttribute('disabled');
                if (btn.getAttribute('data-original-html')) {
                    btn.innerHTML = btn.getAttribute('data-original-html');
                }
            });
        });

    </script>
</asp:Content>

