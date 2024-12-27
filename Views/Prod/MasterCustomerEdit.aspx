<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MasterCustomerEdit.aspx.vb" Inherits="MasterCustomerEdit" MasterPageFile="~/MasterPage.master" Title="DCI | MASTER EDIT CUSTOMER" %>

<asp:Content ContentPlaceHolderID="PageConten" runat="server">
    <div class="content-wrapper">
        <!-- Content Header (Page header) -->
        <section class="content-header">
            <h1>Master Edit Customer
            </h1>
            <ol class="breadcrumb">
                <li><a href="#"><i class="fa fa-gear"></i>Tools</a></li>
                <li class="active">Master Edit Customer</li>
            </ol>
        </section>

        <!-- Main content -->
        <section class="content">

            <div class="box  box-danger">
                <div class="box-header with-border " data-widget="collapse">
                    <h3 class="box-title">Entry Data</h3>
                </div>


                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-horizontal">
                                <div class="box-body">

                                    <div class="form-group">
                                        <label for="inputPassword3" class="col-sm-2 control-label">ID</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtID" class="form-control" runat="server" placeholder="" ReadOnly="true"></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Customer Name</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNama" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">NPWP</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNpwp" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">API</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNoApi" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">NIB</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNIB" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Custom Number</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtCustome" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Account</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtAccount" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Address</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtAddress1" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label"></label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtAddress2" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="box-footer">
                                <asp:button id="cmdAdd" class="btn btn-danger" runat="server" text="Save" />

                                <asp:button id="cmdClear" class="btn btn-default" runat="server" text="Clear" Visible="false" />
                                <asp:button id="cmdBack" class="btn btn-info" runat="server" text="Back" />
                            </div>


                        </div>

                        <div class="col-sm-6 pull-right">
                            <div class="form-horizontal">
                                <div class="box-body">

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Customer Intruction</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtCustomerIntruksi" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">EMAIL</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtEmail" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">POA Receive Date</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtPOA" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Statement Latter Receive Date</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtStatement" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Image On iShare</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtImage" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Expired Date</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtExpired" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Last Update</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtLasteUpdate" class="form-control" runat="server" placeholder="dd/MM/yyyy HH:mm:ss" ReadOnly ="true"></asp:textbox>
                                        </div>
                                    </div>

                                </div>
                            </div>




                        </div>


                    </div>
                    <!--ROW-->
                </div>
            </div>


            <script type="text/javascript" src='https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js'></script>
            <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.4/css/bootstrap-datepicker.css" type="text/css" />
            <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.4/js/bootstrap-datepicker.js" type="text/javascript"></script>
            <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
            <script src="bower_components/bootstrap-3.3.2/bootstrap.min.js"></script>
            <script src="bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
            <script type="text/javascript">
                $(function () {
                    $('[id*=txtExpired]').datepicker({
                        changeMonth: true,
                        changeYear: true,
                        format: "dd/mm/yyyy",
                        language: "tr",
                        orientation: "bottom right"
                    });
                    $('[id*=txtStatement]').datepicker({
                        changeMonth: true,
                        changeYear: true,
                        format: "dd/mm/yyyy",
                        language: "tr",
                        orientation: "bottom right"
                    });
                     $('[id*=txtPOA]').datepicker({
                        changeMonth: true,
                        changeYear: true,
                        format: "dd/mm/yyyy",
                        language: "tr",
                        orientation: "bottom right"
                    });
                });
            </script>


        </section>
        
    </div>
</asp:Content>