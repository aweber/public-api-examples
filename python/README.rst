Python code examples
====================
This directory contains executable examples of how to use
https://api.aweber.com from within Python code.

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

Cloning the repository
~~~~~~~~~~~~~~~~~~~~~~
Before you can use the examples, you need to clone the repository onto your
computer::

   prompt$ git clone https://github.com/aweber/public-api-examples.git

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
can run any example directly::

   prompt (env)$ ./hello-world

You can also run examples using a specific python interpreter::

   prompt$ ./env/bin/python hello-world

.. _Python Packaging User Guide: https://packaging.python.org
.. _venv: https://docs.python.org/3/library/venv.html#module-venv
.. _virtualenv: https://virtualenv.pypa.io/en/stable/

