Python code examples
====================

This directory contains executable examples of how to use
https://api.aweber.com from within Python code.  If you haven't read
`CONTRIBUTING`_, please take a moment to read it now.  It describes the
basic configuration and process and this document picks up where it leaves off.

Quickstart
----------
This repository expects to be run in an isolated virtual environment.  The
following sections describe how to create an isolated environment and install
the required dependencies.  If you already know how to create an isolated
environment, feel free to create an environment and install the requirements
from *requirements.txt*.

If you are not familiar with the Python packaging tools and workflow, it is
described by the `Python Packaging User Guide`_.  The following sections
describe the minimum that you need to do to run the examples.  The
`Python Packaging User Guide`_ provides additional background information on
how packaging works and what each command does.

Python 3 environments
~~~~~~~~~~~~~~~~~~~~~
The code examples are written to run under python 3.  Newer python 2.7
releases should work as well (see the following section).  Python 3 includes
the `venv`_ module so creating an isolated environment does not require
additional packages::

   prompt$ cd public-api-examples/python
   prompt$ python3 -m venv env

Python 2.7 environments
~~~~~~~~~~~~~~~~~~~~~~~
If you are forced to use Python 2.7, then you will need to install
`virtualenv`_ before you can create an isolated work environment::

   prompt$ cd public-api-examples/python
   prompt$ python2.7 -m pip install --user virtualenv
   prompt$ python2.7 -m virtualenv env

Installing requirements
~~~~~~~~~~~~~~~~~~~~~~~
The python examples require a few common packages that we recommend using in
production as well.  They are listed in *requirements.txt* which can be used
to install them into your isolated environment::

   prompt$ . ./env/bin/activate
   prompt (env)$ pip install -r requirements.txt

Running examples
~~~~~~~~~~~~~~~~
Once you have the requirements installed into the isolated environment, you
must create a credentials file.  If you already have a consumer key/secret pair
and access token/secret pair, you can create *credentials.json* in this
directory::

   {
     "client_id": "****",
     "client_secret": "****",
     "token": {
          "access_token": "****",
          "refresh_token": "****"
          "token_type": "bearer",
          "expires_in": 7200,
          "expires_at": 1553268614.907632
     }
   }

You can also create the *credentials.json* file using your existing consumer
key and secret from the `My Apps page`_::

   prompt (env)$ ./get-access-token
   Enter your client id: ****
   Enter your client secret:
   Go to this url: https://auth.aweber.com/oauth2/authorize?oauth_token=****
   Log in and paste the returned URL here: ****
   Updated credentials.json with your new credentials

You can now can run any example directly::

   prompt (env)$ ./some-example

You can also run examples using a specific python interpreter::

   prompt$ ./env/bin/python some-example

Contributing
------------
In addition to the notes in the top-level *CONTRIBUTING* file, we do require
that our python examples pass through both `flake8`_ and `yapf`_ cleanly.
You SHOULD run both utilities locally before you issue a pull request::

   prompt (env)$ flake8 some-example
   prompt (env)$ yapf -d some-example

Both utilities should exit cleanly.

.. _Python Packaging User Guide: https://packaging.python.org
.. _venv: https://docs.python.org/3/library/venv.html#module-venv
.. _virtualenv: https://virtualenv.pypa.io/en/stable/
.. _My Apps page: https://labs.aweber.com/apps
.. _CONTRIBUTING: https://github.com/aweber/public-api-examples/blob/master/CONTRIBUTING.md
.. _flake8: http://flake8.pycqa.org/en/latest/
.. _yapf: https://github.com/google/yapf
