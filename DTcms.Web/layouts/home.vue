<template>
	<div class="main-index">
		<CommonPageHeader></CommonPageHeader>
		<slot />
		<CommonPageFooter></CommonPageFooter>
		<el-backtop :right="30" :bottom="30" />
	</div>
</template>

<style lang="scss">
	.main-index {
		.header {
			top: 0;
			left: 0;
			position: absolute;
			width: 100%;
			z-index: 1;
			color: #fff;
			background: none;
			box-shadow: none;
			a {
				color: #fff;
			}
			.navbar {
				&.el-menu {
					.el-menu-item {
						color: #fff;
					}
					.el-sub-menu__title {
						color: #fff;
					}
					.el-sub-menu {
						&.is-active {
							.el-sub-menu__title {
								color: var(--el-menu-active-color);
							}
						}
					}
				}
			}
			&.scroll {
				color: var(--header-color);
				background: var(--header-bg);
				box-shadow: 0 4px 10px var(--main-shadow);
				a {
					color: var(--header-color);
				}
				.navbar {
					&.el-menu{
						.el-menu-item {
							color: var(--header-color);
						}
						.el-sub-menu__title {
							color: var(--header-color);
						}
					}
				}
			}
		}
	}
</style>