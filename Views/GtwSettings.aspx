<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GtwSettings.aspx.cs" Inherits="OfficialCeisaLite.Views.GtwSettings" MasterPageFile="~/Site.Master"%>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">    

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="col-md-12">
                <div class="nav-tabs-custom">
                    <ul class="nav nav-tabs" style="text-align: left;">
                        <li class="active"><a href="#first" data-toggle="tab" id="first_tab">List Data</a></li>
                    </ul>
    
                    <div class="tab-content">
                        
                        <div id="first" class="tab-pane fade in active">
                            <div class="table-responsive">
                                <asp:UpdatePanel runat="server" ID="UpdatePanelGridView">
                                    <ContentTemplate>                                
                                        <asp:GridView ID="GV_GtwSettings" runat="server"
                                            AllowPaging="true"
                                            AllowSorting="false"
                                            OnRowCommand="GV_GtwSettings_RowCommand"
                                            OnPageIndexChanging="GV_GtwSettings_PageIndexChanging"
                                            PagerStyle-CssClass="arn-pagination"
                                            CssClass="table box table-hover table-striped table-bordered">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-Width="125px">
                                                    <HeaderTemplate>
                                                        <asp:Label Text="Action" runat="server" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnEdit" runat="server"
                                                            CommandArgument='<%# Eval("Gateway") %>'
                                                            CommandName="EditRow"
                                                            CssClass="fa fa-edit fa-2x"></asp:LinkButton>  
                                                        <asp:LinkButton ID="btnDelete" runat="server"
                                                            CommandArgument='<%# Eval("Gateway") %>'
                                                            CommandName="DeleteRow"
                                                            CssClass="fa fa-close fa-2x text-red"
                                                            OnClientClick="if (!confirm('Are you sure you want delete?')) return false;"></asp:LinkButton> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpdatePanelAdd" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:LinkButton ID="btnAdd" CssClass="btn btn-primary" runat="server" OnClick="btnAdd_Click">ADD</asp:LinkButton>
                                    <%-- hide button add --%>
                                    <button type="button" id="btnPopupAdd" data-toggle="modal" data-target="#myModalAdd" style="display: none">
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
    
    <div class="modal" id="myModalAdd">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title" id="modal" runat="server">Add New Settings</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group">
                                        <label for="txtLocation" class="col-sm-3 control-label">Gateway Location</label>

                                        <div class="col-sm-9">
                                            <asp:DropDownList ID="ddlLocation" CssClass="form-control select2" runat="server"></asp:DropDownList>                                                                                      
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtIDPengguna" class="col-sm-3 control-label">ID Pengguna</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtIDPengguna" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtPassword" class="col-sm-3 control-label">Password</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" MaxLength="20" TextMode="Password"></asp:TextBox>
                                        </div>
                                    </div>                                    
                                    <div class="form-group">
                                        <label for="txtNama" class="col-sm-3 control-label">Nama</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtNama" runat="server" CssClass="form-control" MaxLength="100" style="text-transform:uppercase;"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtKota" class="col-sm-3 control-label">Tempat</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtKota" runat="server" CssClass="form-control" MaxLength="50" style="text-transform:uppercase;"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtJabatan" class="col-sm-3 control-label">Jabatan</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtJabatan" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtKodeKantor" class="col-sm-3 control-label">Kode Kantor Pabean</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtKodeKantor" runat="server" CssClass="form-control" MaxLength="6"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtKodeGudang" class="col-sm-3 control-label">Kode Gudang</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtKodeGudang" runat="server" CssClass="form-control" MaxLength="4" style="text-transform:uppercase;"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtKodeAju" class="col-sm-3 control-label">Kode Nomor Pengajuan</label>

                                        <div class="col-sm-9">
                                            <asp:TextBox ID="txtKodeAju" runat="server" CssClass="form-control" MaxLength="12"></asp:TextBox>                                               
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button id="btnClose" type="button" class="btn btn-default pull-left" data-dismiss="modal">Close</button>
                            <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" UseSubmitBehavior="false"/>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
