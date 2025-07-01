<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Candidate_Registration.aspx.cs" Inherits="Registration_Form.Candidate_Registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section id="main-content">

    <section id="wrapper">
        <div class="row">
            <div class="col-lg-12">
                <section class="panel">
                    <header class="panel-heading">
                        <div class="col-md-4 col-md-offset-4">
                            <h1>Registration</h1>
                        </div>
                    </header>


                    <div class="panel-body">
                        <div class="row">
                            <div class="col-md-4 col-md-offset-1">
                                <div class="form-group">
                                    <asp:Label Text="First Name" runat="server"  />
                                    <asp:TextBox ID="txtFirstName" runat="server" Enabled="true" CssClass="form-control input-sm" placeholder="Enter your First Name" />
                                </div>

                                </div>

                             <div class="col-md-4 col-md-offset-1">
                             <div class="form-group">
                <asp:Label Text="Last Name" runat="server"  />
                <asp:TextBox ID="txtLastName" runat="server" Enabled="true" CssClass="form-control input-sm" placeholder="Enter your Last Name" />
                 </div>
                 </div>
                  </div>


                  <div class="row">
                 <div class="col-md-4 col-md-offset-1">
                    <div class="form-group">
                        <asp:Label Text="Address" runat="server"  />
                        <asp:TextBox ID="txtAddress" runat="server" Enabled="true" CssClass="form-control input-sm" placeholder="Enter your address" />
                    </div>

                   </div>

                 <div class="col-md-4 col-md-offset-1">
                 <div class="form-group">
                    <asp:Label Text="DOB" runat="server"  />
                    <asp:TextBox ID="txtDOB" runat="server" Enabled="true" TextMode="Date" CssClass="form-control input-sm" placeholder="DOB" />
                </div>

                </div>
                </div>

              <div class="row">
            <div class="col-md-4 col-md-offset-1">
            <div class="form-group">
                <asp:Label Text="Email" runat="server"  />
                 <asp:TextBox ID="txtEmail" runat="server" Enabled="true" CssClass="form-control input-sm" placeholder="Enter your email" />
             </div>

            </div>

           <div class="col-md-4 col-md-offset-1">
          <div class="form-group">
           <asp:Label Text="Mobile Number" runat="server"  />
          <asp:TextBox ID="txtMobile" runat="server" Enabled="true" CssClass="form-control input-sm" placeholder="Mobile Number 10 Digit Only" />
          </div>
           </div>
            </div>
            <div class="row">
                <div class="col-md-8 col-md-offset-2">
                    <asp:HiddenField ID="hdnID" runat="server" />
                    <div style="height: 15vh; display: flex; justify-content: center; align-items: center;">
                    <asp:Button Text="SUBMIT" ID="btnsubmit" CssClass="btn btn-primary" Width="170px" runat="server" BackColor="#33CCFF" ForeColor="Black" OnClick="btnsubmit_Click" />
                    <asp:Button Text="UPDATE" ID="btnsubmit1" CssClass="btn btn-primary" Width="170px" runat="server" BackColor="Lime" ForeColor="Black" OnClick="btnsubmit1_Click" />
                  
                    <%--<asp:Button Text="SEARCH" ID="btnsubmit3" CssClass="btn btn-primary" Width="170px" runat="server" BackColor="Yellow" ForeColor="Black" OnClick="btnsubmit3_Click" />--%>
                   
               </div>
                        <div style="width: 80%; margin: auto;">
          <%--<asp:GridView ID="grdCanDtls" runat="server" DataKeyNames="id">
    <Columns>

        <asp:BoundField DataField="fname" HeaderText="FirstName" Visible="False" ReadOnly="True" />
        <asp:BoundField DataField="lname" HeaderText="LastName" Visible="False" ReadOnly="True"  />
        <asp:BoundField DataField="address" HeaderText="Address" Visible="False" ReadOnly="True"  />

        <asp:BoundField DataField="dob" HeaderText="DOB" Visible="False" ReadOnly="True" />

        <asp:BoundField DataField="email" HeaderText="Email" Visible="False" ReadOnly="True" />
        <asp:BoundField DataField="mobnum" HeaderText="MobileNum" Visible="False" ReadOnly="True"  />

       <asp:TemplateField HeaderText= "Actions" ShowHeader="True">
            <ItemTemplate>
                <asp:LinkButton ID="Delete" runat="server" DataKeyNames="id" CommandName="DeleteRow" CommandArgument='<%# Eval("id") %>>' OnClick = "Delete_Click" > Delete </asp:LinkButton>
                <asp:LinkButton ID="Edit" runat="server" DataKeyNames="id" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>' OnClick = "Edit_Click" > Edit </asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:LinkButton ID="Update" runat="server" DataKeyNames="id" CommandName="UpdateRow" CommandArgument='<%# Container.DataItemIndex %>' OnClick = "Update_Click" > Update  </asp:LinkButton>
                    <asp:LinkButton ID="Cancel" runat="server" DataKeyNames="id" CommandName="CancelRow" CommandArgument='<%# Container.DataItemIndex %>' OnClick = "Cancel_Click" > Cancel  </asp:LinkButton>
                </EditItemTemplate>
            
        </asp:TemplateField>

    </Columns>
           </asp:GridView>--%>
             <asp:GridView ID="grdCanDtls" runat="server" AutoGenerateColumns="False" DataKeyNames="id" 
    OnRowEditing="grdCanDtls_RowEditing"
    OnRowUpdating="grdCanDtls_RowUpdating"
    OnRowCancelingEdit="grdCanDtls_RowCancelingEdit"
    OnRowDeleting="grdCanDtls_RowDeleting">

    <Columns>
        <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" />

        <asp:TemplateField HeaderText="First Name">
            <ItemTemplate>
                <%# Eval("fname") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditFName" runat="server" Text='<%# Bind("fname") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Name">
            <ItemTemplate>
                <%# Eval("lname") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditLName" runat="server" Text='<%# Bind("lname") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Address">
            <ItemTemplate>
                <%# Eval("address") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditAddress" runat="server" Text='<%# Bind("address") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="DOB">
            <ItemTemplate>
                <%# Convert.ToDateTime(Eval("dob")).ToString("dd/MM/yyyy") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditDOB" runat="server" Text='<%# Bind("dob", "{0:yyyy-MM-dd}") %>' TextMode="Date" />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("email") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditEmail" runat="server" Text='<%# Bind("email") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Mobile">
            <ItemTemplate>
                <%# Eval("mobnum") %>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtEditMobile" runat="server" Text='<%# Bind("mobnum") %>' />
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Actions">
            <ItemTemplate>
                <%--<asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="Edit" />--%>
                
                <asp:LinkButton ID="LinkButton1" runat="server" Text="Edit" CommandArgument='<%# Eval("id") %>' OnClick="Edit_Click" />


                &nbsp;|&nbsp;
                <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="Delete" />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" Text="Update" />
                &nbsp;|&nbsp;
                <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" Text="Cancel" />

                <%--<asp:LinkButton ID="Cancel" runat="server" OnClick="Cancel_Click">Cancel</asp:LinkButton>--%>

            </EditItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            </div>
               </div>
            </div>
            
           </div>
             </section>
            </div>
            
              </div>

            </section>
        </section>

</asp:Content>

