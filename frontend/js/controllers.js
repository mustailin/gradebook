'use strict';

var controllers = angular.module('GradebookControllers', []);

function pad(num, size) {
    var s = num + "";
    while (s.length < size)
        s = "0" + s;
    return s;
}

var serverTimeZoneOffset = 4;

controllers.controller('MainCtrl', function($scope, $http, $rootScope, $window,
    $mdDialog, $mdSidenav, $location, AuthenticationService, MessageService,
    Gradebook, Administrator, Lecturer, Student) {

    $scope.index = 0;
    $scope.toggleSidenav = function(menuId) {
        $mdSidenav(menuId).toggle();
    };

    $scope.closeSidenav = function(menuId) {
        $mdSidenav(menuId).close();
    };

    $scope.logout = function() {
        AuthenticationService.Logout();
        $location.path('/login');
    };

    try {
        var userRole = $rootScope.globals.currentUser.userRole;

        if (userRole === 'admin') {
            $scope.isHidden = true;
            $scope.isAdmin = true;
            $scope.menuTitle = 'ADMINISTRATIVE TOOLS';

            var admin = Administrator.get({
                administratorId: $rootScope.globals.currentUser.userId
            }, function() {
                $scope.user = {
                    firstName: admin.firstName,
                    lastName: admin.lastName
                };
            });
        }

        if (userRole === 'lecturer') {
            $scope.menuTitle = 'GROUPS';

            var lecturer = Lecturer.get({
                lecturerId: $rootScope.globals.currentUser.userId
            }, function() {
                $scope.user = {
                    firstName: lecturer.firstName,
                    lastName: lecturer.lastName
                };
            });

            $scope.userGradebooks = Gradebook.query();
        }

        if (userRole === 'student') {
            $scope.menuTitle = 'GROUPS';
            $scope.isHidden = true;

            var student = Student.get({
                studentId: $rootScope.globals.currentUser.userId
            }, function() {
                $scope.user = {
                    firstName: student.firstName,
                    lastName: student.lastName
                };
            });

            $scope.userGradebooks = Gradebook.query();
        }
    } catch (error) {

    }

    function getByGradebookId(arr, id) {
        for (var d = 0, len = arr.length; d < len; d += 1) {
            if (arr[d].gradebookId === id) {
                return arr[d];
            }
        }
    }

    $scope.$on('selectTab', function(event, index) {
        $scope.selectedIndex = index;
    });

    $scope.get = function(gradebookId) {
        var currentGradebook = getByGradebookId($scope.userGradebooks, gradebookId);
        $scope.gradebook = {
            group: currentGradebook.number + " |",
            subject: currentGradebook.subject + " |",
            year: currentGradebook.academicYear,
            semester: currentGradebook.name
        };
    };


    $scope.showAddGradebookDialog = function(ev) {
        $mdDialog.show({
            controller: 'AddGradebookDialogController',
            templateUrl: 'views/partitials/dialog.gradebook.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                lecturerId: $rootScope.globals.currentUser.userId
            }
        }).then(function() {
            $window.location.reload();
            var message = 'New gradebook was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showGradebookDetailsDialog = function(ev, gradebookId) {
        $mdDialog.show({
            controller: 'GradebookDetailsDialogController',
            templateUrl: 'views/partitials/dialog.gradebook-details.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                gradebookId: gradebookId
            }
        });
    };
});

controllers.controller('GradebookDetailsDialogController', function($scope, $mdDialog, gradebookId, Gradebook) {
    $scope.gradebook = Gradebook.get({
        gradebookId: gradebookId
    }, function(gradebook) {
        $scope.group = gradebook.group;
        $scope.semester = gradebook.semester;
        $scope.lecturers = gradebook.lecturerCollection;
    });

    $scope.hide = function() {
        $mdDialog.hide();
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('AddGradebookDialogController', function($scope, $rootScope,
    $mdDialog, Group, Semester, lecturerId, Lecturer, MessageService, Gradebook) {

    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.groupsLoading = true;
    $scope.semestersLoading = true;
    $scope.lecturersLoading = true;

    $scope.groups = Group.query({}, function() {
        $scope.groupsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.groupsLoading = false;
    });

    $scope.semesters = Semester.query({}, function() {
        $scope.semestersLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.semestersLoading = false;
    });

    $scope.lecturer;
    $scope.searchText = null;
    $scope.selectedLecturers = [];

    Lecturer.query(function(lecturers) {

        if ($rootScope.globals.currentUser.userRole === 'lecturer') {

            var currentLecturer = {};
            for (var i = 0; i < lecturers.length; i++) {
                if (lecturers[i].id === lecturerId)
                    currentLecturer = lecturers[i];
            }
            $scope.selectedLecturers.push(currentLecturer);
        }

        $scope.lecturers = lecturers.map(function(lect) {
            lect._lowerFirstName = lect.firstName.toLowerCase();
            lect._lowerLastName = lect.lastName.toLowerCase();
            return lect;
        });
    }).$promise.then(function() {
        $scope.lecturersLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.lecturersLoading = false;
    });

    $scope.querySearch = function(query) {
        var results = query ? $scope.lecturers.filter(createFilterFor(query)) : [];
        return results;
    };

    function createFilterFor(query) {
        var lowercaseQuery = angular.lowercase(query);
        return function filterFn(lecturer) {
            return (lecturer._lowerFirstName.indexOf(lowercaseQuery) === 0) ||
                (lecturer._lowerLastName.indexOf(lowercaseQuery) === 0);
        };
    }

    $scope.addLecturer = function() {
        if ($scope.lecturer !== undefined) {

            var lecturer = $scope.lecturer;

            if ($scope.selectedLecturers.length !== 0) {
                for (var i = 0; i < $scope.selectedLecturers.length; i++) {
                    if ($scope.selectedLecturers[i].id === $scope.lecturer.id) {
                        lecturer = undefined;
                    }
                }
                if (lecturer !== undefined) {
                    $scope.selectedLecturers.push(lecturer);
                    $scope.lecturer = undefined;
                } else {
                    $scope.lecturer = undefined;
                }
            } else {
                $scope.selectedLecturers.push(lecturer);
                $scope.lecturer = undefined;
            }
        }
    };

    $scope.clearSelection = function() {
        $scope.lecturer = undefined;
    };

    $scope.submit = function() {
        if ($scope.gradebookForm.$valid) {

            $scope.dataLoading = true;

            var selectedLecturerIds = [];

            for (var i = 0; i < $scope.selectedLecturers.length; i++) {
                selectedLecturerIds.push($scope.selectedLecturers[i].id);
            }

            var gradebook = $scope.gradebook;
            gradebook.lecturerCollection = selectedLecturerIds;

            var newGradebook = new Gradebook(gradebook);

            newGradebook.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('LoginCtrl', function($scope, $rootScope, $mdToast, $location, AuthenticationService) {
    // reset login status
    AuthenticationService.Logout();

    $scope.login = function() {
        if ($scope.loginForm.$valid) {
            $scope.dataLoading = true;
            AuthenticationService.Login($scope.username, $scope.password, function(response) {
                if (response.status === 200) {
                    $location.path('/');
                } else {
                    showErrorToast();
                    $scope.dataLoading = false;
                }
            });
        }
    };
    var showErrorToast = function() {
        $mdToast.show(
            $mdToast.simple()
            .content('Wrong username or password. Please try again!')
            .position("top right")
            .hideDelay(3000)
        );
        $scope.password = "";
    };
});

controllers.controller('GradebookTasksCtrl', function($scope, $rootScope, $state, $stateParams, $mdDialog, MessageService, GradebookTasks) {
    $scope.groupGrades = GradebookTasks.query({
        groupId: $stateParams.groupId,
        semesterId: $stateParams.semesterId,
        gradebookId: $stateParams.gradebookId
    });

    var userRole = $rootScope.globals.currentUser.userRole;
    if (userRole === 'student') {
        $scope.isHidden = true;
        $scope.isDisabled = true;
    }

    $scope.orderByName = function(student) {
        return student[0].firstName;
    };

    $scope.calcTaskDates = function(date, taskLength) {
        var startDate = new Date(date);
        startDate.setDate(startDate.getDate() - startDate.getDay() + 1);

        var endDate = new Date(startDate);
        endDate.setDate(endDate.getDate() + (taskLength * 7) - 1);

        return pad(startDate.getDate(), 2) + '/' + pad(startDate.getMonth() + 1, 2) + ' - ' +
            pad(endDate.getDate(), 2) + '/' + pad(endDate.getMonth() + 1, 2);
    };

    $scope.showAddTaskForm = function(ev) {
        $mdDialog.show({
            controller: 'AddTaskDialogController',
            templateUrl: 'views/partitials/dialog.task-form.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                gradebookId: $stateParams.gradebookId
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New task was successfully added to gradebook!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showTaskDetails = function(ev, taskId) {
        $mdDialog.show({
            controller: 'TaskDetailsDialogController',
            templateUrl: 'views/partitials/dialog.task-details.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                taskId: taskId
            }
        });
    };

    $scope.showEditGradeDialog = function(ev, task) {
        $mdDialog.show({
            controller: 'EditGradeDialogController',
            templateUrl: 'views/partitials/dialog.grade.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                task: task
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'The Grade was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };
});

controllers.controller('AddTaskDialogController', function($scope, $mdDialog,
    $rootScope, MessageService, Gradebook, Task, gradebookId) {

    $scope.mode = '????????????';
    $scope.submitButton = '??????????????????????';

    $scope.userRole = $rootScope.globals.currentUser.userRole;

    if ($scope.userRole === 'admin') {
        $scope.gradebooksLoading = true;

        $scope.gradebooks = Gradebook.query({}, function() {
            $scope.gradebooksLoading = false;
        }, function() {
            MessageService.showErrorToast();
            $scope.gradebooksLoading = false;
        });
    }

    $scope.submit = function() {
        if ($scope.taskForm.$valid) {
            $scope.dataLoading = true;

            var newTask = new Task($scope.task);

            if ($scope.userRole === 'lecturer') {
                newTask.gradebookId = parseInt(gradebookId);
            }

            newTask.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('TaskDetailsDialogController', function($scope, $rootScope, $state, $mdDialog, MessageService, taskId, Task) {
    $scope.task = Task.get({ taskId: taskId }, function(task) {

        var startDate = new Date(task.startDate);
        startDate.setDate(startDate.getDate() - startDate.getDay() + 1);
        $scope.startDate = pad(startDate.getDate(), 2) + '/' +
            pad(startDate.getMonth() + 1, 2) + '/' +
            startDate.getFullYear();

        var endDate = new Date(startDate);
        endDate.setDate(endDate.getDate() + (task.taskLength * 7) - 1);
        $scope.endDate = pad(endDate.getDate(), 2) + '/' +
            pad(endDate.getMonth() + 1, 2) + '/' +
            endDate.getFullYear();

        var onCourseDays = [];
        if (task.onCourseMon)
            onCourseDays.push("Monday");
        if (task.onCourseTue)
            onCourseDays.push("Tuesday");
        if (task.onCourseWed)
            onCourseDays.push("Wednesday");
        if (task.onCourseThu)
            onCourseDays.push("Thursday");
        if (task.onCourseFri)
            onCourseDays.push("Friday");

        $scope.onCourseDays = onCourseDays;
    });

    $scope.showEditTaskForm = function(ev) {
        $mdDialog.show({
            controller: 'EditTaskDialogController',
            templateUrl: 'views/partitials/dialog.task-form.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                task: $scope.task
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });

            var message = 'The task was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.hide = function() {
        $mdDialog.hide();
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };

    var userRole = $rootScope.globals.currentUser.userRole;
    if (userRole === 'student') {
        $scope.isHidden = true;
    }
});

controllers.controller('EditTaskDialogController', function($scope, $mdDialog,
    MessageService, task, Task) {

    $scope.mode = '????????????????????';
    $scope.submitButton = '??????????????';

    $scope.task = task;
    var startDate = new Date(task.startDate);
    startDate.setUTCHours(serverTimeZoneOffset); // Server Timezone patch
    $scope.task.startDate = new Date(startDate);

    $scope.submit = function() {
        if ($scope.taskForm.$valid) {
            $scope.dataLoading = true;

            Task.update({
                    taskId: task.id
                }, $scope.task,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditGradeDialogController', function($scope, $mdDialog, MessageService, task, Grade) {

    $scope.task = task;

    var studentGrade = {};
    studentGrade.grade = task.grade;

    $scope.studentGrade = studentGrade;

    $scope.update = function() {
        if ($scope.gradeForm.$valid) {
            $scope.dataLoading = true;

            Grade.update({
                    studentId: task.studentId,
                    taskId: task.taskId
                }, $scope.studentGrade,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});


controllers.controller('GradebookAttendanceCtrl', function($scope, $rootScope, $state, $stateParams, $mdDialog, GradebookAttendance, MessageService) {
    $scope.groupAttendance = GradebookAttendance.query({
        groupId: $stateParams.groupId,
        semesterId: $stateParams.semesterId,
        gradebookId: $stateParams.gradebookId
    });

    var userRole = $rootScope.globals.currentUser.userRole;
    if (userRole === 'student') {
        $scope.isDisabled = true;
    }

    $scope.orderByName = function(student) {
        return student[0].firstName;
    };

    $scope.getDayOfWeek = function(d) {
        var date = new Date(d);

        var weekday = new Array(7);
        weekday[0] = "Sun";
        weekday[1] = "Mon";
        weekday[2] = "Tue";
        weekday[3] = "Wed";
        weekday[4] = "Thu";
        weekday[5] = "Fri";
        weekday[6] = "Sat";

        return weekday[date.getDay()];
    };

    $scope.formatClassDate = function(d) {
        var date = new Date(d);
        return pad(date.getDate(), 2) + '/' + pad(date.getMonth() + 1, 2);
    };

    $scope.attendanceStatus = function(scope) {
        var attendance = scope.attendance;
        if (attendance.present) {
            scope.present = true;
            return 'check_box';
        } else if (attendance.absent) {
            scope.absent = true;
            return 'indeterminate_check_box';
        } else if (attendance.absentWithReason) {
            scope.absentWithReason = true;
            return 'indeterminate_check_box';
        } else {
            return 'check_box_outline_blank';
        }
    };

    $scope.showEditAttendanceDialog = function(ev, attendance) {
        $mdDialog.show({
            controller: 'EditAttendanceDialogController',
            templateUrl: 'views/partitials/dialog.attendance.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                attendance: attendance
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'The Attendance was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showTaskDetails = function(ev, taskId) {
        $mdDialog.show({
            controller: 'TaskDetailsDialogController',
            templateUrl: 'views/partitials/dialog.task-details.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                taskId: taskId
            }
        });
    };
});

controllers.controller('EditAttendanceDialogController', function($scope, $mdDialog, MessageService, attendance, Attendance) {

    $scope.attendance = attendance;

    var classDate = new Date(attendance.classDate);
    classDate.setUTCHours(serverTimeZoneOffset); // Server Timezone patch
    $scope.classDate = pad(classDate.getDate(), 2) + '/' +
        pad(classDate.getMonth() + 1, 2) + '/' +
        classDate.getFullYear();

    if (attendance.present) {
        $scope.status = 'present';
    } else if (attendance.absent) {
        $scope.status = 'absent';
    } else if (attendance.absentWithReason) {
        $scope.status = 'absent-with-reason';
    }

    $scope.update = function() {
        $scope.dataLoading = true;

        var attendance = {};
        attendance.attendanceId = $scope.attendance.attendanceId;
        attendance.studentId = $scope.attendance.studentId;
        attendance.taskId = $scope.attendance.taskId;
        attendance.classDate = classDate;
        attendance.present = false;
        attendance.absent = false;
        attendance.absentWithReason = false;

        if ($scope.status === 'present')
            attendance.present = true;
        if ($scope.status === 'absent')
            attendance.absent = true;
        if ($scope.status === 'absent-with-reason')
            attendance.absentWithReason = true;

        Attendance.update({
                attendanceId: attendance.attendanceId
            }, attendance,
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('FacultyCtrl', function($scope, $mdDialog, $state, MessageService, Faculty) {

    $scope.faculties = Faculty.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";


    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }

    };

    $scope.showAddFacultyForm = function(ev) {
        $mdDialog.show({
            controller: 'AddFacultyDialogController',
            templateUrl: 'views/partitials/dialog.faculty.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New faculty was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditFacultyForm = function(ev, faculty) {
        $mdDialog.show({
            controller: 'EditFacultyDialogController',
            templateUrl: 'views/partitials/dialog.faculty.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                faculty: faculty
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Faculty was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteFacultyDialog = function(ev, faculty) {
        $mdDialog.show({
            controller: 'DeleteFacultyDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                faculty: faculty
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Faculty was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };

});

controllers.controller('AddFacultyDialogController', function($scope, $mdDialog, MessageService, Faculty) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.submit = function() {
        if ($scope.facultyForm.$valid) {
            $scope.dataLoading = true;

            var newFaculty = new Faculty($scope.faculty);

            newFaculty.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditFacultyDialogController', function($scope, $mdDialog, MessageService, faculty, Faculty) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.faculty = faculty;

    $scope.submit = function() {
        if ($scope.facultyForm.$valid) {
            $scope.dataLoading = true;

            Faculty.update({
                    facultyId: faculty.id
                }, $scope.faculty,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteFacultyDialogController', function($scope, $mdDialog, MessageService, faculty) {
    $scope.object = 'Faculty';

    $scope.submit = function() {

        $scope.dataLoading = true;

        faculty.$delete({
                facultyId: faculty.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });

    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DepartmentCtrl', function($scope, $mdDialog, $state, MessageService, Department) {

    $scope.departments = Department.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";


    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }

    };

    $scope.showAddDepartmentForm = function(ev) {
        $mdDialog.show({
            controller: 'AddDepartmentDialogController',
            templateUrl: 'views/partitials/dialog.department.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New department was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditDepartmentForm = function(ev, department) {
        $mdDialog.show({
            controller: 'EditDepartmentDialogController',
            templateUrl: 'views/partitials/dialog.department.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                department: department
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Department was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteDepartmentDialog = function(ev, department) {
        $mdDialog.show({
            controller: 'DeleteDepartmentDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                department: department
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Department was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };

});

controllers.controller('AddDepartmentDialogController', function($scope, $mdDialog, MessageService, Department, Faculty) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.facultiesLoading = true;

    $scope.faculties = Faculty.query({}, function() {
        $scope.facultiesLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.facultiesLoading = false;
    });

    $scope.submit = function() {
        if ($scope.departmentForm.$valid) {
            $scope.dataLoading = true;

            var newDepartment = new Department($scope.department);

            newDepartment.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditDepartmentDialogController', function($scope, $mdDialog, MessageService, department, Faculty, Department) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.department = department;
    $scope.facultiesLoading = true;

    $scope.faculties = Faculty.query({}, function() {
        $scope.facultiesLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.facultiesLoading = false;
    });

    $scope.submit = function() {
        if ($scope.departmentForm.$valid) {
            $scope.dataLoading = true;

            Department.update({
                    departmentId: department.id
                }, $scope.department,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteDepartmentDialogController', function($scope, $mdDialog, MessageService, department) {
    $scope.object = 'Department';

    $scope.submit = function() {

        $scope.dataLoading = true;

        department.$delete({
                departmentId: department.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });

    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('SemesterCtrl', function($scope, $mdDialog, $state, MessageService, Semester) {

    $scope.semesters = Semester.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.search;


    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }

    };

    $scope.showAddSemesterForm = function(ev) {
        $mdDialog.show({
            controller: 'AddSemesterDialogController',
            templateUrl: 'views/partitials/dialog.semester.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New semester was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditSemesterForm = function(ev, semester) {
        $mdDialog.show({
            controller: 'EditSemesterDialogController',
            templateUrl: 'views/partitials/dialog.semester.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                semester: semester
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Semester was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteSemesterDialog = function(ev, semester) {
        $mdDialog.show({
            controller: 'DeleteSemesterDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                semester: semester
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Semester was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };

});

controllers.controller('AddSemesterDialogController', function($scope, $mdDialog, MessageService, Semester) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.submit = function() {
        if ($scope.semesterForm.$valid) {
            $scope.dataLoading = true;

            var newSemester = new Semester($scope.semester);

            newSemester.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditSemesterDialogController', function($scope, $mdDialog, MessageService, semester, Semester) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.semester = semester;

    $scope.submit = function() {
        if ($scope.semesterForm.$valid) {
            $scope.dataLoading = true;

            Semester.update({
                    semesterId: semester.id
                }, $scope.semester,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteSemesterDialogController', function($scope, $mdDialog, MessageService, semester) {
    $scope.object = 'Semester';

    $scope.submit = function() {

        $scope.dataLoading = true;

        semester.$delete({
                semesterId: semester.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });

    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('LecturerCtrl', function($scope, $mdDialog, $state, MessageService, Lecturer) {

    $scope.lecturers = Lecturer.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }

    };

    $scope.showAddLecturerForm = function(ev) {
        $mdDialog.show({
            controller: 'AddLecturerDialogController',
            templateUrl: 'views/partitials/dialog.lecturer.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New lecturer was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditLecturerForm = function(ev, lecturer) {
        $mdDialog.show({
            controller: 'EditLecturerDialogController',
            templateUrl: 'views/partitials/dialog.lecturer.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                lecturer: lecturer
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Lecturer was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteLecturerDialog = function(ev, lecturer) {
        $mdDialog.show({
            controller: 'DeleteLecturerDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                lecturer: lecturer
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Lecturer was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };

});

controllers.controller('AddLecturerDialogController', function($scope, $mdDialog, MessageService, Department, Lecturer) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.departmentsLoading = true;

    $scope.departments = Department.query({}, function() {
        $scope.departmentsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.departmentsLoading = false;
    });

    $scope.submit = function() {
        if ($scope.lecturerForm.$valid) {
            $scope.dataLoading = true;

            var newLecturer = new Lecturer($scope.lecturer);

            newLecturer.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditLecturerDialogController', function($scope, $mdDialog, MessageService, Department, lecturer, Lecturer) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.lecturer = lecturer;

    $scope.departmentsLoading = true;

    $scope.departments = Department.query({}, function() {
        $scope.departmentsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.departmentsLoading = false;
    });

    $scope.submit = function() {
        if ($scope.lecturerForm.$valid) {
            $scope.dataLoading = true;

            Lecturer.update({
                    lecturerId: lecturer.id
                }, $scope.lecturer,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteLecturerDialogController', function($scope, $mdDialog, MessageService, lecturer) {
    $scope.object = 'Lecturer';

    $scope.submit = function() {

        $scope.dataLoading = true;

        lecturer.$delete({
                lecturerId: lecturer.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('StudentCtrl', function($scope, $mdDialog, $state, MessageService, Student) {

    $scope.students = Student.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }
    };

    $scope.showAddStudentForm = function(ev) {
        $mdDialog.show({
            controller: 'AddStudentDialogController',
            templateUrl: 'views/partitials/dialog.student.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New student was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditStudentForm = function(ev, student) {
        $mdDialog.show({
            controller: 'EditStudentDialogController',
            templateUrl: 'views/partitials/dialog.student.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                student: student
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Student was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteStudentDialog = function(ev, student) {
        $mdDialog.show({
            controller: 'DeleteStudentDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                student: student
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Student was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };
});

controllers.controller('AddStudentDialogController', function($scope, $mdDialog, MessageService, Group, Student) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.groupsLoading = true;

    $scope.groups = Group.query({}, function() {
        $scope.groupsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.groupsLoading = false;
    });

    $scope.submit = function() {
        if ($scope.studentForm.$valid) {
            $scope.dataLoading = true;

            var newStudent = new Student($scope.student);

            newStudent.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditStudentDialogController', function($scope, $mdDialog, MessageService, Group, student, Student) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.student = student;

    $scope.groupsLoading = true;

    $scope.groups = Group.query({}, function() {
        $scope.groupsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.groupsLoading = false;
    });

    $scope.submit = function() {
        if ($scope.studentForm.$valid) {
            $scope.dataLoading = true;

            Student.update({
                    studentId: student.id
                }, $scope.student,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteStudentDialogController', function($scope, $mdDialog, MessageService, student) {
    $scope.object = 'Student';

    $scope.submit = function() {

        $scope.dataLoading = true;

        student.$delete({
                studentId: student.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('AdministratorCtrl', function($scope, $mdDialog, $state, MessageService, Administrator) {

    $scope.administrators = Administrator.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }
    };

    $scope.showAddAdministratorForm = function(ev) {
        $mdDialog.show({
            controller: 'AddAdministratorDialogController',
            templateUrl: 'views/partitials/dialog.administrator.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New administrator was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditAdministratorForm = function(ev, administrator) {
        $mdDialog.show({
            controller: 'EditAdministratorDialogController',
            templateUrl: 'views/partitials/dialog.administrator.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                administrator: administrator
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Administrator was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteAdministratorDialog = function(ev, administrator) {
        $mdDialog.show({
            controller: 'DeleteAdministratorDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                administrator: administrator
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Administrator was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };
});

controllers.controller('AddAdministratorDialogController', function($scope, $mdDialog, MessageService, Administrator) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.submit = function() {
        if ($scope.administratorForm.$valid) {
            $scope.dataLoading = true;

            var newAdministrator = new Administrator($scope.administrator);

            newAdministrator.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditAdministratorDialogController', function($scope, $mdDialog, MessageService, administrator, Administrator) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.administrator = administrator;

    $scope.submit = function() {
        if ($scope.administratorForm.$valid) {
            $scope.dataLoading = true;

            Administrator.update({
                    administratorId: administrator.id
                }, $scope.administrator,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteAdministratorDialogController', function($scope, $mdDialog, MessageService, administrator) {
    $scope.object = 'Administrator';

    $scope.submit = function() {

        $scope.dataLoading = true;

        administrator.$delete({
                administratorId: administrator.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('GroupCtrl', function($scope, $mdDialog, $state, MessageService, Group) {

    $scope.groups = Group.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }

    };

    $scope.showAddGroupForm = function(ev) {
        $mdDialog.show({
            controller: 'AddGroupDialogController',
            templateUrl: 'views/partitials/dialog.group.html',
            parent: angular.element(document.body),
            targetEvent: ev
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New group was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditGroupForm = function(ev, group) {
        $mdDialog.show({
            controller: 'EditGroupDialogController',
            templateUrl: 'views/partitials/dialog.group.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                group: group
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Group was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteGroupDialog = function(ev, group) {
        $mdDialog.show({
            controller: 'DeleteGroupDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                group: group
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Group was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };

});

controllers.controller('AddGroupDialogController', function($scope, $mdDialog, MessageService, Group, Faculty) {
    $scope.mode = 'Add';
    $scope.submitButton = 'Submit';

    $scope.facultiesLoading = true;

    $scope.faculties = Faculty.query({}, function() {
        $scope.facultiesLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.facultiesLoading = false;
    });

    $scope.submit = function() {
        if ($scope.groupForm.$valid) {
            $scope.dataLoading = true;

            var newGroup = new Group($scope.group);

            newGroup.$save({}, function() {
                $mdDialog.hide();
            }, function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('EditGroupDialogController', function($scope, $mdDialog, MessageService, group, Faculty, Group) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.group = group;
    $scope.facultiesLoading = true;

    $scope.faculties = Faculty.query({}, function() {
        $scope.facultiesLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.facultiesLoading = false;
    });

    $scope.submit = function() {
        if ($scope.groupForm.$valid) {
            $scope.dataLoading = true;

            Group.update({
                    groupId: group.id
                }, $scope.group,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteGroupDialogController', function($scope, $mdDialog, MessageService, group) {
    $scope.object = 'Group';

    $scope.submit = function() {

        $scope.dataLoading = true;

        group.$delete({
                groupId: group.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });

    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('GradebookCtrl', function($scope, $mdDialog, $state, MessageService, Gradebook) {

    $scope.gradebooks = Gradebook.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }
    };

    $scope.showAddGradebookForm = function(ev) {
        $mdDialog.show({
            controller: 'AddGradebookDialogController',
            templateUrl: 'views/partitials/dialog.gradebook.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                lecturerId: null
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New gradebook was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditGradebookForm = function(ev, gradebook) {
        $mdDialog.show({
            controller: 'EditGradebookDialogController',
            templateUrl: 'views/partitials/dialog.gradebook.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                gradebook: gradebook
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Gradebook was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteGradebookDialog = function(ev, gradebook) {
        $mdDialog.show({
            controller: 'DeleteGradebookDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                gradebook: gradebook
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Gradebook was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };
});

controllers.controller('EditGradebookDialogController', function($scope, $mdDialog, MessageService, gradebook, Group, Semester, Lecturer, Gradebook) {
    $scope.mode = 'Edit';
    $scope.submitButton = 'Update';

    $scope.gradebook = gradebook;

    $scope.groupsLoading = true;
    $scope.semestersLoading = true;
    $scope.lecturersLoading = true;

    $scope.groups = Group.query({}, function() {
        $scope.groupsLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.groupsLoading = false;
    });

    $scope.semesters = Semester.query({}, function() {
        $scope.semestersLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.semestersLoading = false;
    });

    $scope.lecturer;
    $scope.searchText = null;
    $scope.selectedLecturers = [];

    Lecturer.query(function(lecturers) {
        for (var i = 0; i < gradebook.lecturerCollection.length; i++) {
            for (var j = 0; j < lecturers.length; j++) {
                if (gradebook.lecturerCollection[i].id === lecturers[j].id)
                    $scope.selectedLecturers.push(lecturers[j]);
            }
        }

        $scope.lecturers = lecturers.map(function(lect) {
            lect._lowerFirstName = lect.firstName.toLowerCase();
            lect._lowerLastName = lect.lastName.toLowerCase();
            return lect;
        });
    }).$promise.then(function() {
        $scope.lecturersLoading = false;
    }, function() {
        MessageService.showErrorToast();
        $scope.lecturersLoading = false;
    });

    $scope.querySearch = function(query) {
        var results = query ? $scope.lecturers.filter(createFilterFor(query)) : [];
        return results;
    };

    function createFilterFor(query) {
        var lowercaseQuery = angular.lowercase(query);
        return function filterFn(lecturer) {
            return (lecturer._lowerFirstName.indexOf(lowercaseQuery) === 0) ||
                (lecturer._lowerLastName.indexOf(lowercaseQuery) === 0);
        };
    }

    $scope.addLecturer = function() {
        if ($scope.lecturer !== undefined) {

            var lecturer = $scope.lecturer;

            if ($scope.selectedLecturers.length !== 0) {
                for (var i = 0; i < $scope.selectedLecturers.length; i++) {
                    if ($scope.selectedLecturers[i].id === $scope.lecturer.id) {
                        lecturer = undefined;
                    }
                }
                if (lecturer !== undefined) {
                    $scope.selectedLecturers.push(lecturer);
                    $scope.lecturer = undefined;
                } else {
                    $scope.lecturer = undefined;
                }
            } else {
                $scope.selectedLecturers.push(lecturer);
                $scope.lecturer = undefined;
            }
        }
    };

    $scope.clearSelection = function() {
        $scope.lecturer = undefined;
    };

    $scope.submit = function() {
        if ($scope.gradebookForm.$valid) {
            $scope.dataLoading = true;

            var selectedLecturerIds = [];

            for (var i = 0; i < $scope.selectedLecturers.length; i++) {
                selectedLecturerIds.push({ lecturerId: $scope.selectedLecturers[i].id });
            }

            var gradebook = $scope.gradebook;
            gradebook.lecturerCollection = selectedLecturerIds;

            Gradebook.update({
                    gradebookId: gradebook.id
                }, gradebook,
                function() {
                    $mdDialog.hide();
                },
                function() {
                    MessageService.showErrorToast();
                    $scope.dataLoading = false;
                });
        }
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('DeleteGradebookDialogController', function($scope, $mdDialog, MessageService, gradebook) {
    $scope.object = 'Gradebook';

    $scope.submit = function() {

        $scope.dataLoading = true;

        gradebook.$delete({
                gradebookId: gradebook.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});

controllers.controller('TaskCtrl', function($scope, $mdDialog, $state, MessageService, Task) {

    $scope.tasks = Task.query();

    $scope.isDeletionEnabled = false;
    $scope.status = "????????????????";

    $scope.isExpanded = false;

    $scope.toggleFilters = function() {
        var isExpanded = $scope.isExpanded;
        $scope.isExpanded = !isExpanded;
    };

    $scope.onChange = function(isDeletionEnabled) {
        if (isDeletionEnabled) {
            $scope.status = "??????????????????";
        } else {
            $scope.status = "????????????????";
        }
    };

    $scope.showAddTaskForm = function(ev) {
        $mdDialog.show({
            controller: 'AddTaskDialogController',
            templateUrl: 'views/partitials/dialog.task-form.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                gradebookId: null
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'New task was successfully added!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showEditTaskForm = function(ev, task) {
        $mdDialog.show({
            controller: 'EditTaskDialogController',
            templateUrl: 'views/partitials/dialog.task-form.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                task: task
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Task was successfully updated!';
            MessageService.showSuccessToast(message);
        });
    };

    $scope.showDeleteTaskDialog = function(ev, task) {
        $mdDialog.show({
            controller: 'DeleteTaskDialogController',
            templateUrl: 'views/partitials/dialog.delete-confirmation.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            locals: {
                task: task
            }
        }).then(function() {
            $state.go($state.current, {}, { reload: true });
            var message = 'Task was successfully deleted!';
            MessageService.showSuccessToast(message);
        });
    };
});

controllers.controller('DeleteTaskDialogController', function($scope, $mdDialog, MessageService, task) {
    $scope.object = 'Task';

    $scope.submit = function() {

        $scope.dataLoading = true;

        task.$delete({
                taskId: task.id
            },
            function() {
                $mdDialog.hide();
            },
            function() {
                MessageService.showErrorToast();
                $scope.dataLoading = false;
            });
    };

    $scope.cancel = function() {
        $mdDialog.cancel();
    };
});