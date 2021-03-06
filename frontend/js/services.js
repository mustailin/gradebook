'use strict';

var services = angular.module('GradebookServices', []);

var oauthServiceHostname = 'https://localhost:44392';
var gradebookServiceHostname = 'https://localhost:44389';

services.factory('Gradebook', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/gradebooks/:gradebookId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Group', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/groups/:groupId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Semester', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/semesters/:semesterId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Administrator', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/administrators/:administratorId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Lecturer', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/lecturers/:lecturerId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Student', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/students/:studentId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('GradebookTasks', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/groups/:groupId/semesters/:semesterId/gradebooks/:gradebookId/tasks');
});

services.factory('GradebookAttendance', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/groups/:groupId/semesters/:semesterId/gradebooks/:gradebookId/attendance');
});

services.factory('Task', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/tasks/:taskId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Grade', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/students/:studentId/tasks/:taskId/grade', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Attendance', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/attendances/:attendanceId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Faculty', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/faculties/:facultyId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('Department', function($resource) {
    return $resource(gradebookServiceHostname + '/api/v1/departments/:departmentId', null, {
        update: { method: 'PUT' }
    });
});

services.factory('MessageService', function($mdToast) {
    return {
        showSuccessToast: function(message) {
            $mdToast.show(
                $mdToast.simple()
                .content(message)
                .position("top right")
                .hideDelay(3000)
            );
        },
        showErrorToast: function() {
            $mdToast.show(
                $mdToast.simple()
                .content('Something went wrong! Please try again!')
                .position("top right")
                .hideDelay(3000)
            );
        }
    };
});

services.factory('AuthenticationService', function(Base64, $http, $cookieStore, $rootScope, $timeout) {
    var service = {};


    service.Login = function(username, password, callback) {
        var authdata = Base64.encode(username + ':' + password);
        var token;

        var callUserCheck = function(response) {
            $http.get(gradebookServiceHostname + '/api/v1/users/check', {
                headers: { 'Authorization': 'Bearer ' + token }
            }).then(function(response) {
                setCurrentUserDetails(response);
                callback(response);
            }, function(response) {
                callback(response);
            });
        }

        var setCurrentUserDetails = function(response) {
            $rootScope.globals = {
                currentUser: {
                    userId: response.data.id,
                    username: username,
                    token: token,
                    userRole: response.data.role
                }
            };
            $http.defaults.headers.common['Authorization'] = 'Bearer ' + token;
            $cookieStore.put('globals', $rootScope.globals);
        }

        $http.get(oauthServiceHostname + '/oauth/authorize?client_id=gradebook-web&response_type=token&redirect_uri=http://localhost', {
            headers: { 'Authorization': 'Basic ' + authdata }
        }).then(function(response) {
            token = response.headers('token');
            callUserCheck(response);
        }, function(response) {
            callback(response);
        });
    };

    service.Logout = function() {
        $rootScope.globals = {};
        $cookieStore.remove('globals');
        $http.defaults.headers.common.Authorization = 'Bearer ';
    };

    return service;
});

services.factory('Base64', function() {

    var keyStr = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';

    return {
        encode: function(input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            do {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);

                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;

                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }

                output = output +
                    keyStr.charAt(enc1) +
                    keyStr.charAt(enc2) +
                    keyStr.charAt(enc3) +
                    keyStr.charAt(enc4);
            } while (i < input.length);

            return output;
        },
        decode: function(input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
            var base64test = /[^A-Za-z0-9\+\/\=]/g;
            if (base64test.exec(input)) {
                window.alert("There were invalid base64 characters in the input text.\n" +
                    "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                    "Expect errors in decoding.");
            }
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

            do {
                enc1 = keyStr.indexOf(input.charAt(i++));
                enc2 = keyStr.indexOf(input.charAt(i++));
                enc3 = keyStr.indexOf(input.charAt(i++));
                enc4 = keyStr.indexOf(input.charAt(i++));

                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;

                output = output + String.fromCharCode(chr1);

                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }

            } while (i < input.length);

            return output;
        }
    };
});