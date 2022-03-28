
var pg = {
    pageSize: 10,
    pageIndex: 1
}

var pgchucvu = {
    pageSize: 10,
    pageIndex: 1
}

var pgrole = {
    pageSize: 10,
    pageIndex: 1
}
//Thông báo
var Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000
});

//Main
var userController = {
    init: function () {
        //Khởi tạo theme
        userController.initTheme();

        userController.listAccount();

        userController.createAccount();

        userController.dschucvu();

        userController.createchucvu();

        userController.lstroleall();

    },
    initTheme: function () {
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    },
    actionDisplayProduct: function () {
        var flag = false;
        $('.actiondisplayProduct').off('click').on('click', function () {
            var id = $(this).data('id');
            console.log('da click' + id);
            if (flag == false) {
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-play fas fa-pause")
                flag = true;
            }
            else {
                $('#actiondisplay').hide('1000');
                $('#actiondisplay').toggle('1000');
                icon = $(this).find("i");
                icon.toggleClass("fas fa-pause fas fa-play")
                flag = false;
            }

            $.ajax({
                type: "get",
                url: '/admin/user/ChangeUser',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data.Check == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thành công',
                            })
                            userController.listAccount();
                        } else {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thành công',
                            })
                            userController.listAccount();
                        }
                    }
                }
            });


        })
    },
    listAccount: function (changePageSize) {
        $.ajax({
            url: '/admin/user/GetAllCategoryNewPaging',
            type: 'get',
            data: {
                page: pg.pageIndex,
                pageSize: pg.pageSize
            },
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateNews').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            UserIDDisplay: 'NV' + ChangeIDProduct(value.UserID),
                            UserID: value.UserID,
                            FullName: value.FullName,
                            UserName: value.UserName,
                            CreateDate: value.CreateDate!=null?parseJsonDate(value.CreateDate):"",
                            Address: value.Address,
                            Phone: value.Phone,
                            Birth: value.Birth!=null?parseJsonDate(value.Birth):"",
                            UserRoleGroup: value.UserRoleGroup,
                            DisplayProduct: value.UserStatus == 1 ? "fas fa-pause" : "fas fa-play",
                            UserStatus: value.UserStatus == 1 ? "<span class=\"badge bg-primary\">Hoạt động</span>" : "<span class=\"badge bg-danger\">Khóa</span>"
                        });
                    });
                    $('#tbNews').html(html);
                    if (data.total != 0) {
                        userController.paging(data.total, function () {
                            userController.listAccount();
                        }, changePageSize)
                    }

                    userController.actionDisplayProduct();
                    userController.editcategorynew();
                    userController.lstchucvuedit();
                    userController.deletenews();
                }
            }
        });
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getDate().toString().padStart(2, '0')}/${(dt.getMonth() + 1).toString().padStart(2, '0')} /${dt.getFullYear().toString().padStart(4, '0')} ${dt.getHours().toString().padStart(2, '0')}: ${dt.getMinutes().toString().padStart(2, '0')}: ${dt.getSeconds().toString().padStart(2, '0')}`
        }
    },
    createAccount: function () {
        
        $("#CreateNews").off('click').on('click', function () {

            userController.lstchucvu();
            $("#ModalCreateNewsCategory").modal('show');

            $("#FromCreateNewsCategory").off('submit').on('submit', function (e) {
                e.preventDefault();


                var user = new Object();
                user.UserName = $("#UserName").val();
                user.Passsword = $("#Passsword").val();
                user.FullName = $("#FullName").val();
                user.Address = $("#Address").val();
                user.Phone = $("#Phone").val();
                user.Birth = $("#Birth").val();
                user.UserRoleGroupID = $("#lstchucvu").val();

                $.ajax({
                    type: "post",
                    url: '/admin/user/CreatUser',
                    data: user,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data.Check == true) {
                                $("#ModalCreateNewsCategory").modal('hide');
                                $("#UserName").val('');
                                $("#Passsword").val('');
                                $("#FullName").val('');
                                $("#Address").val('');
                                $("#Phone").val('');
                                $("#Birth").val('');
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                userController.listAccount();
                                

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: data.data.Result,
                                })
                            }
                        }
                    }
                });

            })
        })
    },
    lstchucvu: function () {
        $.ajax({
            url: '/admin/user/GetChucVuAll',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#lstchucvu');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UserRoleGroupID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    },
    editcategorynew: function () {
        
        $(".modalshowEdit").off('click').on('click', function () {
            
            var id = $(this).data('id');

            $.ajax({
                url: '/admin/user/GetUserByID',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {

                        $("#UserIDEdit").val('NV' + ChangeIDProduct(id));

                        $("#UserNameEdit").val(data.data.UserName);

                      

                        $("#FullNameEdit").val(data.data.FullName);

                        $("#AddressEdit").val(data.data.Address);

                        $("#PhoneEdit").val(data.data.Phone);

                        $("#BirthEdit").val(parseJsonDate(data.data.Birth));

               

                        $(`#lstchucvuEdit option[value='${data.data.UserRoleGroupID}']`).prop('selected', true);


                        $("#ModalEditNewsCategory").modal('show');
                    }
                }
            });

            $("#FromEditNewsCategory").off('submit').on('submit', function (e) {

                e.preventDefault();
           
                var user = new Object();
                user.UserID = id;
                user.UserName = $("#UserNameEdit").val();
                user.Passsword = $("#PassswordEdit").val();
                user.FullName = $("#FullNameEdit").val();
                user.Address = $("#AddressEdit").val();
                user.Phone = $("#PhoneEdit").val();
                user.Birth = $("#BirthEdit").val();
                user.UserRoleGroupID = $("#lstchucvuEdit").val();

                $.ajax({
                    type: "post",
                    url: '/admin/user/UpdateUser',
                    data: user,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data == true) {

                                
                                $("#ModalEditNewsCategory").modal('hide');

                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                userController.listAccount();
                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: 'Cập nhật Thất bại',
                                })
                            }
                        }
                    }
                });
            })

        })
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }


        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}`
        }
    },
    lstchucvuedit: function () {
        $.ajax({
            url: '/admin/user/GetChucVuAll',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var $select = $('#lstchucvuEdit');
                    $select.find('option').remove();
                    $.each(data.data, function (key, val) {
                        $('<option>').val(val.UserRoleGroupID).text(val.Name).appendTo($select);
                    });
                }
            }
        });
    },
    deletenews: function () {
        $(".actiondelete").off('click').on('click', function () {
            $("#ModalDeleteProduct").modal('show');

            var id = $(this).data('id');
            var name = $(this).data('name');


            $("#NameProductDelete").empty();
            $("#NameProductDelete").append('Bạn có chắc chắn xóa: ' + 'NV' + ChangeIDProduct(id) + 'Tên nhân viên: ' + name)

            $("#btndeleteProduct").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/user/DeleteUser',
                    type: 'get',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data.Check == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Xóa thành công',
                                })
                                $('#ModalDeleteProduct').modal('hide')
                                userController.listAccount();

                            } else {
                                $("#ResultProductDelete").empty();
                                $("#ResultProductDelete").removeAttr("hidden");
                                $("#ResultProductDelete").append(data.data.Result);
                            }
                        }
                    }
                });
            })


        })
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }
    },
    paging: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pg.pageSize)
        if ($('#paginationnews a').length === 0 || changePageSize === true) {
            $('#paginationnews').empty();
            $('#paginationnews').removeData('twbs-pagination');
            $('#paginationnews').unbind("page");
        }
        $('#paginationnews').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pg.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    },

    dschucvu: function (changePageSize) {

        $.ajax({
            url: '/admin/user/GetChucVu',
            type: 'get',
            data: {
                page: pgchucvu.pageIndex,
                pageSize: pgchucvu.pageSize
            },
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templateChucvu').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            UserRoleGroupID: value.UserRoleGroupID,
                            UserRoleGroupIDDisplay: 'RL' + ChangeIDProduct(value.UserRoleGroupID),
                            Name: value.Name,
                            CreateDate: parseJsonDate(value.CreateDate),
                       });
                    });
                    $('#tbChucvu').html(html);
                    if (data.total != 0) {
                        userController.pagingchucvu(data.total, function () {
                            userController.dschucvu();
                        }, changePageSize)
                    }

                    userController.editcategorynew();
                    userController.deletechucvu();
                }
            }
        });
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }
       
        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}`
        }
    }
    ,
    pagingchucvu: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgchucvu.pageSize)
        if ($('#paginationchucvu a').length === 0 || changePageSize === true) {
            $('#paginationchucvu').empty();
            $('#paginationchucvu').removeData('twbs-pagination');
            $('#paginationchucvu').unbind("page");
        }
        $('#paginationchucvu').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgchucvu.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    },


    createchucvu: function () {
        userController.lstrole();
        $("#CreateChucvu").off('click').on('click', function () {
            $("#ModalCreateChucvu").modal('show');


            $("#FromCreateChucvu").off('submit').on('submit', function (e) {
                e.preventDefault();

                var selected = [];

                $('div.listrole input[type=checkbox]').each(function () {
                    if ($(this).is(":checked")) {
                        selected.push($(this).val());
                    }
                });

                $.ajax({
                    type: "post",
                    url: '/admin/user/UpdateChucVu',
                    data: {
                        role: selected,
                        name: $("#NameRoleGroup").val()

                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data.Check == true) {

                                $("#ModalCreateChucvu").modal('hide');

                                $("#NameRoleGroup").val('');
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                userController.lstchucvu();

                                userController.dschucvu();

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: data.data.Result,
                                })
                            }
                        }
                    }

                })

            })

          
        })
    }
    ,
    lstrole: function () {

        $.ajax({
            url: '/admin/user/GetQuyen',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var select = $('.listrole');
                    select.empty();
                    $.each(data.data, function (key, val) {
                        select.append('<div class="col-2"><div class="form-check"><input class="form-check-input" type="checkbox" value="' + val.UserRoleID + '" id="Check' + val.UserRoleID + '"><label class="form-check-label">' + val.Name +'</label></div></div >')
                    });
                }
            }
        });

    }
    ,
    editcategorynew: function () {
        $(".modalshowEditCategory").off('click').on('click', function () {
            userController.lstroleedit();
            var id = $(this).data('id');

            var Name = $(this).data('name');

            $("#IDChucVu").val('RL' + ChangeIDProduct(id))

            $("#NameChucVu").val(Name)

            $.ajax({
                url: '/admin/user/GetRoleByID',
                type: 'get',
                data: {
                    id: id
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code = 200) {

                        console.log(data.data)

                        $.each(data.data, function (k, v) {

                            
                            $('#Checkedit' + v).prop('checked', true);
                           
                        })
                        

                        $("#ModalEditChucvu").modal('show');
                    }
                }
            });

            $("#FromEditChucvu").off('submit').on('submit', function (e) {

                e.preventDefault();

                var selected = [];

                $('div.listroleedit input[type=checkbox]').each(function () {
                    if ($(this).is(":checked")) {
                        selected.push($(this).val());
                    }
                });


                var t = new Object();
                t.UserRoleGroupID = id;
                t.Name = $("#NameChucVu").val();


                $.ajax({
                    type: "post",
                    url: '/admin/user/UpdateChucVu',
                    data: {
                        role: selected,
                        userRoleGroup: t

                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 200) {
                            if (data.data.Check == true) {

                                $("#ModalEditChucvu").modal('hide');

                                $("#NameChucVu").val('');
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Thành công',
                                })
                                userController.dschucvu();
                                userController.lstchucvu();

                            } else {
                                Toast.fire({
                                    icon: 'error',
                                    title: data.data.Result,
                                })
                            }
                        }
                    }

                })
            })

        })
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }
    },
    lstroleedit: function () {

        $.ajax({
            url: '/admin/user/GetQuyen',
            type: 'get',
            dataType: 'json',
            success: function (data) {
                if (data.code = 200) {
                    var select = $('.listroleedit');
                    select.empty();
                    $.each(data.data, function (key, val) {
                        select.append('<div class="col-2"><div class="form-check"><input class="form-check-input" type="checkbox" value="' + val.UserRoleID + '" id="Checkedit' + val.UserRoleID + '"><label class="form-check-label">' + val.Name + '</label></div></div >')
                    });
                }
            }
        });

    },

    lstroleall: function (changePageSize) {

        $.ajax({
            url: '/admin/user/GetQuyenALL',
            type: 'get',
            data: {
                page: pgrole.pageIndex,
                pageSize: pgrole.pageSize
            },
            success: function (data) {
                if (data.code = 200) {
                    var html = "";
                    var template = $('#data-templaterole').html();
                    $.each(data.data, function (key, value) {
                        html += Mustache.render(template, {
                            STT: value.STT,
                            UserRoleID: value.UserRoleID,
                            UserRoleIDDisplay: 'RLS' + ChangeIDProduct(value.UserRoleID),
                            Name: value.Name,
                            CreateDate: parseJsonDate(value.CreateDate),
                        });
                    });
                    $('#tbrole').html(html);
                    if (data.total != 0) {
                        userController.pagingrple(data.total, function () {
                            userController.lstroleall();
                        }, changePageSize)
                    }
                }
            }
        });
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }

        function parseJsonDate(jsonDate) {

            var dt = new Date(parseInt(jsonDate.substr(6)));

            return `${dt.getFullYear().toString().padStart(4, '0')}-${(dt.getMonth() + 1).toString().padStart(2, '0')}-${dt.getDate().toString().padStart(2, '0')}`
        }

    },
    pagingrple: function (totalRow, callback, changePageSize) {
        var totalPage = Math.ceil(totalRow / pgrole.pageSize)
        if ($('#paginationrole a').length === 0 || changePageSize === true) {
            $('#paginationrole').empty();
            $('#paginationrole').removeData('twbs-pagination');
            $('#paginationrole').unbind("page");
        }
        $('#paginationrole').twbsPagination({
            totalPages: totalPage,
            visiblePages: 7,
            first: 'Đầu',
            next: 'Tiếp',
            last: 'Cuối',
            prev: 'Trước',
            onPageClick: function (event, page) {
                pgrole.pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    },

    deletechucvu: function () {
        $(".actiondeleteChucvu").off('click').on('click', function () {
            $("#ModalDeleteChucVu").modal('show');

            var id = $(this).data('id');
            var name = $(this).data('name');

            $("#NameCVDelete").empty();
            $("#NameCVDelete").append('Bạn có chắc chắn xóa: ' + 'RL' + ChangeIDProduct(id) + 'Tên chức vụ: ' + name)

            $("#btndeletechucvu").off('click').on('click', function (e) {
                e.preventDefault()
                $.ajax({
                    url: '/admin/user/DeleteChucVu',
                    type: 'get',
                    data: {
                        id: id
                    },
                    dataType: 'json',
                    success: function (data) {
                        if (data.code = 200) {
                            if (data.data.Check == true) {
                                Toast.fire({
                                    icon: 'success',
                                    title: 'Xóa thành công',
                                })
                                $('#ModalDeleteChucVu').modal('hide')
                                userController.dschucvu();

                            } else {
                                $("#ResultCVDelete").empty();
                                $("#ResultCVDelete").removeAttr("hidden");
                                $("#ResultCVDelete").append(data.data.Result);
                            }
                        }
                    }
                });
            })


        })
        function ChangeIDProduct(id) {
            if (id <= 9) {
                return '000' + id;
            } else if (id <= 99) {
                return '00' + id;
            } else if (id <= 999) {
                return '0' + id;
            } else {
                return id;
            }
        }
    },
}
userController.init();

