<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditBarang.aspx.cs" Inherits="OfficialCeisaLite.Views.EditBarang" %>

<asp:Content ID="FilterContent" ContentPlaceHolderID="Head" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Always" runat="server">
        <ContentTemplate>
        
        <div class="col-md-12">            
            <div class="box box-primary">
                <div class="box-header with-border">
                    <h3 class="box-title">HAWB &nbsp<span runat="server" id="lblHAWB" class="label pull-right bg-aqua"></span></h3>

                    <div class="box-tools pull-right">
                        <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                        </button>
                    </div>
                </div>

                <div class="box-body">
                    <div class="table-responsive">
                        <asp:UpdatePanel ID="grdUpdatePanel" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <%--<asp:GridView ID="GV_EditBarang" runat="server"
                                    AllowPaging="true"
                                    AllowSorting="false"
                                    AutoGenerateColumns="false"
                                    OnPageIndexChanging="GV_EditBarang_PageIndexChanging"
                                    OnPageIndexChanged="GV_EditBarang_PageIndexChanged"
                                    OnRowCommand="GV_EditBarang_RowCommand"
                                    PagerStyle-CssClass="arn-pagination"
                                    CssClass="table box table-hover table-striped table-bordered">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Edit">                                           
                                            <ItemTemplate> 
                                                <asp:LinkButton runat="server" ID="cmdEdit" CommandName="Edit">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-edit"></span>
                                                    EDIT
                                                </asp:LinkButton>                                                        
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton CssClass="btn btn-primary" ID="cmdUpdate" runat="server" OnClick="cmdUpdate_Click" CommandName="Update" Text="Update">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-saved"></span>
                                                </asp:LinkButton>
                                                <asp:LinkButton CssClass="btn btn-danger" ID="cmdCancel" runat="server" OnClick="cmdCancel_Click" CommandName="Cancel" Text="Cancel">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-lock"></span>
                                                </asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle Width="100px" HorizontalAlign="center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="HAWB" HeaderText="HAWB" ReadOnly="True" Visible="true"/>
                                        <asp:BoundField DataField="TGL_HAWB" HeaderText="TGL HAWB" ReadOnly="True" Visible="true"/>
                                        <asp:BoundField DataField="ASURANSI" HeaderText="ASURANSI" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="CIF" HeaderText="CIF" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="CIF RUPIAH" HeaderText="CIF Rupiah" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="FOB" HeaderText="FOB" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="FREIGHT" HeaderText="FREIGHT" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="HARGA SATUAN" HeaderText="Harga Satuan" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="JUMLAH SATUAN" HeaderText="Jumlah Satuan" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="KODE NEGARA ASAL" HeaderText="Kd Negara" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="KODE SATUAN BARANG" HeaderText="Kd Satuan" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="JUMLAH KEMASAN" HeaderText="Jumlah Kemasan" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="KODE KEMASAN" HeaderText="Kd Kemasan" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="KODE BARANG" HeaderText="Kd Barang" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="NDPBM" HeaderText="Kurs" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="NETTO" HeaderText="Netto" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="KATEGORI BARANG" HeaderText="Kategori Barang" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="POS TARIF" HeaderText="HS CODE" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="BM TARIF" HeaderText="BM" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="PPN TARIF" HeaderText="PPN" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="PPH TARIF" HeaderText="PPH" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="URAIAN" HeaderText="Uraian" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="MERK" HeaderText="Merk" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="TIPE" HeaderText="Tipe" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="UKURAN" HeaderText="Ukuran" ReadOnly="false" Visible="true"/>
                                        <asp:BoundField DataField="SPESIFIKASI LAINNYA" HeaderText="Spek Lainnya" ReadOnly="false" Visible="true"/> 
                                        
                                    </Columns>
                                </asp:GridView>--%>
                                <asp:GridView ID="GV_Barang" runat="server"
                                    AllowPaging="true"
                                    AllowSorting="false"
                                    PageSize="10"
                                    PagerStyle-CssClass="arn-pagination"
                                    OnPageIndexChanging="GV_Barang_PageIndexChanging"
                                    OnRowEditing="GV_Barang_RowEditing"
                                    OnRowUpdating="GV_Barang_RowUpdating"
                                    OnRowCancelingEdit="GV_Barang_RowCancelingEdit"
                                    CssClass="table box table-hover table-striped table-bordered"
                                    DataKeyNames="ID">
                                    <Columns>
                                        <asp:TemplateField HeaderText="ACTION">                                           
                                            <ItemTemplate> 
                                                <asp:LinkButton runat="server" ID="cmdEdit" CommandName="Edit">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-edit"></span> <strong>EDIT</strong>
                                                </asp:LinkButton>                                                        
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton CssClass="btn btn-primary" ID="cmdUpdate" runat="server" OnClick="cmdUpdate_Click" OnClientClick="if (!confirm('Are you sure want to UPDATE data?')) return false;" CommandName="Update">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-saved" title="UPDATE"></span>
                                                </asp:LinkButton>
                                                <asp:LinkButton CssClass="btn btn-danger" ID="cmdCancel" runat="server" OnClick="cmdCancel_Click" OnClientClick="if (!confirm('Are you sure want to CANCEL EDIT data?')) return false;" CommandName="Cancel">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-lock" title="CANCEL"></span>
                                                </asp:LinkButton>
                                                <asp:LinkButton CssClass="btn btn-success" ID="cmdCopy" runat="server" OnClick="cmdCopy_Click" OnClientClick="if (!confirm('Are you sure want to COPY data?')) return false;" CommandName="Copy">
                                                    <span aria-hidden="true" class="glyphicon glyphicon-copy" title="COPY"></span>
                                                </asp:LinkButton>
                                            </EditItemTemplate>
                                            <HeaderStyle Width="100px" HorizontalAlign="center" VerticalAlign="Middle"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                             </ContentTemplate>
                        </asp:updatePanel>   
                        <asp:LinkButton ID="btnSubmitData" CssClass="btn btn-primary" runat="server" OnClick="btnSubmitData_Click" OnClientClick="if (!confirm('Are you sure want to SAVE data?')) return false;">Save Data</asp:LinkButton>
                        
                    </div> 
                </div>
                
            </div>
        </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
