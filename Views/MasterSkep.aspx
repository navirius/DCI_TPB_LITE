<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterSkep.aspx.cs" Inherits="OfficialCeisaLite.Views.MasterSkep" MasterPageFile="~/Site.Master"%>

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
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">NPWP</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcNPWP" runat="server" CssClass="form-control"></asp:TextBox>                                               
                                        </div>
                                    </div> &nbsp;
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">Nama Customer</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="srcNama" runat="server" CssClass="form-control"></asp:TextBox>                                               
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="col-md-12">
                <div class="nav-tabs-custom">
                    <ul class="nav nav-tabs" style="text-align: left;">
                        <li class="active"><a href="#first" data-toggle="tab" id="first_tab">List Data &nbsp<span runat="server" id="totalData" class="label pull-right bg-red"></span></a></li>
                    </ul>
    
                    <div class="tab-content">
                        <div id="first" class="tab-pane fade in active">
                            <div class="table-responsive">
                                <asp:UpdatePanel runat="server" ID="UpdatePanelGridView">
                                    <ContentTemplate>                                
                                        <asp:GridView ID="GV_MasterSkep" runat="server"
                                            AllowPaging="true"
                                            AllowSorting="false"
                                            OnRowCommand="GV_MasterSkep_RowCommand"
                                            OnPageIndexChanging="GV_MasterSkep_PageIndexChanging"
                                            PagerStyle-CssClass="arn-pagination"
                                            CssClass="table box table-hover table-striped table-bordered">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="5px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnEdit" runat="server"
                                                            CommandArgument='<%# Eval("ID") %>'
                                                            CommandName="EditRow"
                                                            CssClass="fa fa-edit fa-2x"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpdatePanelExport" UpdateMode="Always">
                                    <ContentTemplate>
                                        <%--<asp:LinkButton ID="btnAdd" CssClass="btn btn-primary" runat="server" OnClick="btnAdd_Click">ADD</asp:LinkButton>
                                        <asp:LinkButton ID="btnExport" CssClass="btn btn-success pull-right" runat="server" OnClick="btnExport_Click">Export to Excel</asp:LinkButton>--%>
                                        <%-- hide button add --%>
                                        <button type="button" id="btnPopup" data-toggle="modal" data-target="#myModal"
                                            class="btn btn-primary btn-sm" style="display: none">
                                        </button>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>                        
                    </div>
                </div>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div class="modal" id="myModal">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Edit Data SKEP</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">NAMA CUSTOMER</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtNamaCust" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">NO SKEP</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtNoSkep" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label class="col-sm-3 control-label">TGL SKEP</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtTglSkep" runat="server" CssClass="form-control" MaxLength="10" placeholder="yyyy-mm-dd"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtLocation" class="col-sm-3 control-label">NIB</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtNoNIB" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>                                                                                     
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtRole" class="col-sm-3 control-label">TGL AKTIF SKEP</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtTglAktif" runat="server" CssClass="form-control" MaxLength="10" placeholder="yyyy-mm-dd"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtFullname" class="col-sm-3 control-label">TGL EXPIRED SKEP</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtTglExpired" runat="server" CssClass="form-control" MaxLength="10" placeholder="yyyy-mm-dd"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnClose" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
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
            $(<%=txtTglSkep.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });
            //Date picker
            $(<%=txtTglAktif.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });  
            //Date picker
            $(<%=txtTglExpired.ClientID%>).datepicker({
                format: 'yyyy-mm-dd',
                autoclose: true
            });  
        })
    }

    </script>

</asp:Content>
