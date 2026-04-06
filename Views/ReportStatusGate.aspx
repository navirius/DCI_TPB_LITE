<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportStatusGate.aspx.cs" Inherits="OfficialCeisaLite.Views.ReportStatusGate" %>
<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    <div class="col-md-12">
        <!-- form start -->
            <form id="form1" class="form-horizontal" >
                <div class="box-body">
                    <div class="box-group" id="accordion">
                        <div class="panel box box-primary">
                            <div class="box-header with-border">
                                <h4 class="box-title">
                                    Filter Data
                                </h4>
                            </div>
                            <div class="box-body">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">HAWB</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcHAWB" runat="server" CssClass="form-control" TextMode="MultiLine" style="height:100px;"></asp:TextBox>                                               
                                        </div>
                                    </div> &nbsp;
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">Nama Customer</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcNama" runat="server" CssClass="form-control"></asp:TextBox>                                               
                                        </div>
                                    </div> &nbsp;
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">Tanggal SPPB</label>

                                        <div class="col-sm-9"> 
                                            From:
                                            <asp:TextBox ID="srcTglSppbFrom" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>
                                            To :
                                            <asp:TextBox ID="srcTglSppbTo" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div> &nbsp;
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">Tanggal BC1.1</label>

                                        <div class="col-sm-9"> 
                                            From:
                                            <asp:TextBox ID="srcTglBCFrom" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>
                                            To :
                                            <asp:TextBox ID="srcTglBCTo" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div> &nbsp;
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
                    <h3 class="box-title">Report Data &nbsp<span runat="server" id="totalData" class="label pull-right bg-red"></span></h3>

                    <div class="box-tools pull-right">
                        <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                        </button>
                    </div>
                </div>

                <div class="box-body">
                    <div class="table-responsive">
                        <asp:UpdatePanel ID="grdUpdatePanel" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:GridView ID="GV_ReportData" runat="server"
                                    AllowPaging="true"
                                    AllowSorting="false"
                                    AutoGenerateColumns="false"
                                    OnRowCommand="GV_ReportData_RowCommand"
                                    OnPageIndexChanging="GV_ReportData_PageIndexChanging"
                                    OnPageIndexChanged="GV_ReportData_PageIndexChanged"
                                    PagerStyle-CssClass="arn-pagination"
                                    CssClass="table box table-hover table-striped table-bordered"
                                    PageSize="50"
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
                                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false"/>
                                        <asp:BoundField DataField="Gateway" HeaderText="Gateway" Visible="True"/>
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
                                        <asp:BoundField DataField="SPPB" HeaderText="SPPB" Visible="True"/>                                        
                                        <asp:TemplateField HeaderText="Tgl SPPB">
                                            <ItemTemplate>
                                                <div style="width: 75px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis">
                                                    <%# DataBinder.GetPropertyValue(Container.DataItem, "Tgl SPPB") %>
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
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:updatePanel>
                        <asp:UpdatePanel runat="server" ID="UpdatePanelExport" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:LinkButton ID="btnRefresh" CssClass="btn btn-primary" runat="server" OnClick="btnRefresh_Click">Download Respon Status</asp:LinkButton>
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

    <script>
        function pageLoad() {
        $(function () {

            //Date picker
            $(<%=srcTglBCFrom.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });

            //Date picker
            $(<%=srcTglBCTo.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });

            //Date picker
            $(<%=srcTglSppbFrom.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });

            //Date picker
            $(<%=srcTglSppbTo.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
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
