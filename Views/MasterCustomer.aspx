<%@ Import Namespace="Modul" %>

<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MasterCustomer.aspx.vb" Inherits="MasterGateway" MasterPageFile="~/MasterPage.master" Title="DCI | MASTER CUSTOMER" %>

<asp:Content ContentPlaceHolderID="PageConten" runat="server">
    <div class="content-wrapper">
        <!-- Content Header (Page header) -->
        <section class="content-header">
            <h1>Master Customer
            </h1>
            <ol class="breadcrumb">
                <li><a href="#"><i class="fa fa-gear"></i>Tools</a></li>
                <li class="active">Master Customer</li>
            </ol>
        </section>

        <!-- Main content -->
        <section class="content">

            <div class="box  box-danger    collapsed-box">
                <div class="box-header with-border " data-widget="collapse">
                    <h3 class="box-title">Entry Data</h3>
                </div>


                <div class="box-body">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-horizontal">
                                <div class="box-body">
                                    <div class="form-group hidden">
                                        <label for="inputPassword3" class="col-sm-2 control-label">ID</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtID" class="form-control" runat="server" placeholder=""></asp:textbox>
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
                                        <label for="" class="col-sm-2 control-label">Address</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtAddress1" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">City</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtCity" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Contact Name</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtContactName" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Duty Account</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtAccount" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">NIK</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtCustome" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">NIB</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNIB" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">API</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtNoApi" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">SKEP KB</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtSkkb" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">SKB PPh</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtSkbPph" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">POA</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtPOA" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">KITE</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtKemImpTujExp" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">PP 8</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtPP8" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">PP 19</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtPP19" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Others</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtOther" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Statement Date</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtStatement" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="box-footer">
                                <asp:button id="cmdAdd" class="btn btn-danger" runat="server" text="Add / Save" />
                                <asp:button id="cmdClear" class="btn btn-default" runat="server" text="Clear" />
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
                                        <label for="" class="col-sm-2 control-label">Email</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtEmail" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL NPWP</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlImporterId" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Phone No</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtPhoneNo" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Zip Code</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtZipcode" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Mobile Phone</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtMobilePhone" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL NIK</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlNik" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>                                    
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL NIB</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlNib" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL API</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlApi" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL SKEP KB</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlSkkb" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL SKB PPh</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlSkbPph" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL POA</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlPoaDate" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL KITE</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlKemImpTujExp" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL PP 8</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlPP8" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL PP 19</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlPP19" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">URL Other</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtUrlOther" class="form-control" runat="server" placeholder=""></asp:textbox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="" class="col-sm-2 control-label">Expiry Date</label>
                                        <div class="col-sm-10">
                                            <asp:textbox id="txtExpired" class="form-control" runat="server" placeholder="dd/MM/yyyy"></asp:textbox>
                                        </div>
                                    </div>
                                </div>
                            </div>




                        </div>


                    </div>
                    <!--ROW-->
                </div>
            </div>


            <div class="box  box-danger  " style="left: 0px; top: 0px">
                <div class="box-header with-border">
                    <h3 class="box-title">
                        <asp:label id="txtStatus" runat="server" text="Data"></asp:label>
                    </h3>
                    <div class="box-tools">
                        <div class="input-group input-group-sm" style="width: 250px;">
                            <asp:textbox runat="server" id="txtCari" class="form-control pull-right" placeholder="Search"></asp:textbox>
                            <div class="input-group-btn">
                                <asp:linkbutton id="cmdCari" runat="server" text="" class="btn btn-default" onclick="cmdCari_Click">
                                    <i class="fa fa-search"></i>
                                   </asp:linkbutton>
                            </div>
                        </div>
                    </div>
                    
                </div>

                <div class="box-body">
                    <div style="overflow-x: scroll;" class="col-md-12">
                        <asp:GridView ID="GridView1" runat="server" CssClass=" table table-bordered  table-hover table-responsive" AllowPaging="True" AutoGenerateColumns="False" HeaderStyle-BackColor="Silver" PagerStyle-BackColor="Silver">
                            <Columns>
                                <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                                <asp:BoundField DataField="NAMACUSTOMER" HeaderText="Customer Name" />
                                <asp:BoundField DataField="NPWP" HeaderText="NPWP" />
                                <asp:BoundField DataField="NOAPI" HeaderText="API" />
                                <asp:BoundField DataField="NONIB" HeaderText="NIB" />
                                <asp:BoundField DataField="NOCUSTOM" HeaderText="Custom No" />
                                <asp:BoundField DataField="NOACCOUNT" HeaderText="Account No" />
                                <asp:BoundField DataField="ALAMAT1" HeaderText="Addess 1" />
                                <asp:BoundField DataField="INTRUKSICUSOMER" HeaderText="Customer Instruction" />
                                <asp:BoundField DataField="EMAIL" HeaderText="EMail" />
                                <asp:BoundField DataField="TGLPOA" HeaderText="POA Date" />
                                <asp:BoundField DataField="TGLSTATEMENT" HeaderText="Statement Letter Receive Date  " />
                                <%--<asp:BoundField DataField="IMGONSHARE" HeaderText="Image on iShare" />--%>
                                 <asp:HyperLinkField   DataTextField="IMGONSHARE" HeaderText="Image on iShare" DataNavigateUrlFields="IMGONSHARE" />
                                
                                <asp:BoundField DataField="TGLEXPIRED" HeaderText="Expire Date" />
                                <asp:BoundField DataField="UE" HeaderText="Update By" ReadOnly="True" />
                                <asp:BoundField DataField="DE" HeaderText="Last Update" ReadOnly="True" />
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="cmdEdit" runat="server" CssClass="btn btn-primary" OnClick="cmdEdit_Click">
                                        <span aria-hidden="true" class="glyphicon glyphicon-edit"></span>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                           
                                 <HeaderStyle Width="100px" HorizontalAlign="center" VerticalAlign="Middle" />
                                   <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                              <ItemTemplate>
                                  <asp:LinkButton ID="cmdDelete"  runat="server"  CssClass="btn btn-danger" OnClick="cmdDel_Click"    >
                                        <span aria-hidden="true" class="glyphicon glyphicon-remove"></span>
                                   </asp:LinkButton>
                               </ItemTemplate>
                           
                                 <HeaderStyle Width="100px" HorizontalAlign="center" VerticalAlign="Middle" />
                                   <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                               
                   </Columns>
                    <PagerSettings  Mode="NumericFirstLast" FirstPageText="First" PreviousPageText="Previous" NextPageText="Next" LastPageText="Last" />
                            <HeaderStyle BackColor="Silver"></HeaderStyle>
                            <PagerStyle BackColor="Silver"></PagerStyle>

               </asp:gridview>
                    </div>

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
