<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryHSCode.aspx.cs" Inherits="OfficialCeisaLite.Views.QueryHSCode" MasterPageFile="~/Site.Master"%>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
    <div class="col-md-12">
            <!-- form start -->
            <form id="form1" class="form-horizontal">
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
                                            <label class="col-sm-3 control-label">HS CODE</label>

                                            <div class="col-sm-9">
                                                <asp:TextBox ID="srcHSCODE" runat="server" CssClass="form-control" TextMode="MultiLine" style="height:100px;"></asp:TextBox>                                               
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
                                        <asp:GridView ID="GV_HSCode" runat="server"
                                            AllowPaging="true"
                                            AllowSorting="false"
                                            OnPageIndexChanging="GV_HSCode_PageIndexChanging"
                                            PagerStyle-CssClass="arn-pagination"
                                            CssClass="table box table-hover table-striped table-bordered">
                                            <Columns>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel runat="server" ID="UpdatePanelExport" UpdateMode="Always">
                                    <ContentTemplate>
                                        <%--<asp:LinkButton ID="btnAdd" CssClass="btn btn-primary" runat="server" OnClick="btnAdd_Click">ADD</asp:LinkButton>
                                        <asp:LinkButton ID="btnExport" CssClass="btn btn-success pull-right" runat="server" OnClick="btnExport_Click">Export to Excel</asp:LinkButton>--%>
                                        <%-- hide button add --%>
                                        <%--<button type="button" id="btnPopup" data-toggle="modal" data-target="#myModal"
                                            class="btn btn-primary btn-sm" style="display: none">
                                        </button>--%>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>                        
                    </div>
                </div>
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>   
</asp:Content>