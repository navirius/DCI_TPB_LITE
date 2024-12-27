<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="OfficialCeisaLite.Views.UserManagement" MasterPageFile="~/Site.Master"%>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    <div class="col-md-6">
        <div class="box box-primary">
            <div class="box-header with-border">
                <h3 class="box-title">Filter Data</h3>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <div class="form-group">
                    <label for="srcUsername" class="col-sm-3 control-label">Username</label>

                    <div class="col-sm-9">
                        <asp:TextBox ID="srcUsername" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>                                               
                    </div>
                </div> &nbsp
                <div class="form-group">
                    <label for="srcRole" class="col-sm-3 control-label">Role</label>

                    <div class="col-sm-9">
                        <asp:TextBox ID="srcRole" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                    </div>
                </div> &nbsp
            </div>
            <!-- /.box-body -->
                
            <div class="box-footer">
                <asp:UpdatePanel runat="server" ID="UpdatePanelSearch">
                    <ContentTemplate>
                        <asp:LinkButton ID="btnSearch" CssClass="btn btn-primary pull-right" runat="server" OnClick="btnSearch_Click">SEARCH</asp:LinkButton>
                    </ContentTemplate>
                </asp:UpdatePanel>                
            </div>
            <!-- /.box-footer -->
        </div>
        <!-- /.box -->
    </div> 
</asp:Content> 

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="col-md-12">
                <div class="nav-tabs-custom">
                    <ul class="nav nav-tabs" style="text-align: left;">
                        <li class="active"><a href="#first" data-toggle="tab" id="first_tab">List Data</a></li>
                        <li><a href="#second" data-toggle="tab" id="second_tab">List Location</a></li>
                    </ul>
    
                    <div class="tab-content">
                        <div id="first" class="tab-pane fade in active">
                            <div class="table-responsive">
                                <asp:UpdatePanel runat="server" ID="UpdatePanelGridView">
                                    <ContentTemplate>                                
                                        <asp:GridView ID="GV_UserManagement" runat="server"
                                            AllowPaging="true"
                                            AllowSorting="false"
                                            OnRowCommand="GV_UserManagement_RowCommand"
                                            OnPageIndexChanging="GV_UserManagement_PageIndexChanging"
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
                                                <asp:TemplateField ItemStyle-Width="5px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnDelete" runat="server"
                                                            CommandArgument='<%# Eval("ID") %>'
                                                            CommandName="DeleteRow"
                                                            CssClass="fa fa-close fa-2x text-red"
                                                            OnClientClick="if (!confirm('Are you sure you want delete?')) return false;"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpdatePanelExport" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:LinkButton ID="btnAdd" CssClass="btn btn-primary" runat="server" OnClick="btnAdd_Click">ADD</asp:LinkButton>
                                        <asp:LinkButton ID="btnExport" CssClass="btn btn-success pull-right" runat="server" OnClick="btnExport_Click">Export to Excel</asp:LinkButton>
                                        <%-- hide button add --%>
                                        <button type="button" id="btnPopup" data-toggle="modal" data-target="#myModal"
                                            class="btn btn-primary btn-sm" style="display: none">
                                        </button>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>

                        <div id="second" class="tab-pane fade in">
                            <asp:UpdatePanel runat="server" ID="UpdatePanelGridView2">
                                <ContentTemplate>                                
                                    <asp:GridView ID="GV_Location" runat="server"
                                        AllowPaging="true"
                                        AllowSorting="false"
                                        OnPageIndexChanging="GV_Location_PageIndexChanging"
                                        PagerStyle-CssClass="arn-pagination"
                                        CssClass="table box table-hover table-striped table-bordered">                                        
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel runat="server" ID="UpdatePanelLocation" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:LinkButton ID="btnAddLocation" CssClass="btn btn-primary" runat="server" OnClick="btnAddLocation_Click">ADD</asp:LinkButton>
                                    <%-- hide button add --%>
                                    <button type="button" id="btnPopupLocation" data-toggle="modal" data-target="#myModalLocation"
                                        class="btn btn-primary btn-sm" style="display: none">
                                    </button>
                                </ContentTemplate>
                            </asp:UpdatePanel>
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
                            <h4 class="modal-title">Input User Management</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="txtUsername" class="col-sm-3 control-label">UserName</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="20" required="true" placeholder="myname"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtFullname" class="col-sm-3 control-label">FullName</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtFullname" runat="server" CssClass="form-control" MaxLength="50" required="true" placeholder="My Fullname"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtLocation" class="col-sm-3 control-label">Gateway Location</label>

                                        <div class="col-sm-9">
                                            <asp:DropDownList ID="ddlLocation" CssClass="form-control select2" runat="server"></asp:DropDownList>                                                                                      
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtRole" class="col-sm-3 control-label">Role</label>

                                        <div class="col-sm-9">
                                            <asp:DropDownList ID="ddlRole" CssClass="form-control" runat="server"></asp:DropDownList>
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

    <div class="modal" id="myModalLocation">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title">Input Facility Location</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="txtLocationCode" class="col-sm-3 control-label">Location Code</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtLocationCode" runat="server" CssClass="form-control" MaxLength="3" required="true" placeholder="JKT"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="txtLocationName" class="col-sm-3 control-label">Location Name</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtLocationName" runat="server" CssClass="form-control" MaxLength="50" required="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnCloseLocation" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                            <asp:LinkButton ID="btnSaveLocation" Text="Save" runat="server" CssClass="btn btn-primary" OnClick="btnSaveLocation_Click" />
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
            });
        }
    </script>
</asp:Content>